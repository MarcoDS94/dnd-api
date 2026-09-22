using System.Text;
using System.Threading.RateLimiting;
using Dnd.Api.Middleware;
using Dnd.Application.Interfaces;
using Dnd.Infrastructure.Authentication;
using Dnd.Infrastructure.Data;
using Dnd.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

// Allow Npgsql to write UTC DateTime to PostgreSQL 'timestamp without time zone' columns
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Support dynamic PORT binding for Google Cloud Run and container environments (defaults to 8080)
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Configure Kestrel Security & Resource Limits
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.AddServerHeader = false; // Strip 'Server: Kestrel' header
    serverOptions.Limits.MaxRequestBodySize = 2 * 1024 * 1024; // 2MB max request body limit
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();

// Application & Infrastructure Services
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICampaignService, CampaignService>();

// Configure PostgreSQL DbContext with resilient retry strategy
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(3),
            errorCodesToAdd: null)));

// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? "DEVELOPMENT_SECRET_KEY_FOR_JWT_AUTHENTICATION_MIN_32_CHARS!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "DndBackend";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "DndClient";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Configure CORS Policy
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? new[] { "http://localhost:3000", "http://localhost:5173" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Configure Rate Limiting (OWASP Brute-Force & DoS Mitigation)
builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Strict policy for auth endpoints: 5 attempts per 1 minute per IP
    rateLimiterOptions.AddPolicy("auth-policy", httpContext =>
    {
        var clientIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous";
        return RateLimitPartition.GetFixedWindowLimiter(clientIp, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 5,
            Window = TimeSpan.FromMinutes(1),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0
        });
    });

    // General policy for general API endpoints: 100 requests per 1 minute per IP
    rateLimiterOptions.AddPolicy("general-policy", httpContext =>
    {
        var clientIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous";
        return RateLimitPartition.GetSlidingWindowLimiter(clientIp, _ => new SlidingWindowRateLimiterOptions
        {
            PermitLimit = 100,
            Window = TimeSpan.FromMinutes(1),
            SegmentsPerWindow = 4,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0
        });
    });

    rateLimiterOptions.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/problem+json";
        context.HttpContext.Response.Headers.RetryAfter = "60";
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            status = 429,
            title = "Too Many Requests",
            detail = "Rate limit exceeded. Please wait 60 seconds before retrying.",
            instance = context.HttpContext.Request.Path.Value
        }, cancellationToken: token);
    };
});

// Configure HSTS for Production
builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);
});

// Configure Swagger with Bearer Token Authorization
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "D&D Management API",
        Version = "v1",
        Description = "ASP.NET Core Web API with JWT Authentication & Hardened Security"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure Forwarded Headers for Cloud Run / Reverse Proxies (SSL Termination)
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

// 1. Forwarded headers must be processed first for reverse proxies / load balancers
app.UseForwardedHeaders();

// 2. Inject OWASP Security Headers (nosniff, DENY, Referrer-Policy, CSP, Permissions-Policy)
app.UseMiddleware<SecurityHeadersMiddleware>();

// 3. Centralized Global Exception Sanitization (RFC 7807 ProblemDetails, no DB leak)
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

// 4. Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("EnableSwagger", false))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

// Exclude /health from HTTPS redirection so Cloud Run and container probes don't receive a 307 redirect
app.UseWhen(context => !context.Request.Path.StartsWithSegments("/health"), appBuilder =>
{
    appBuilder.UseHttpsRedirection();
});

// 5. CORS policy
app.UseCors("CorsPolicy");

// 6. Rate Limiting
app.UseRateLimiter();

// 7. Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// 8. Health checks & Controller endpoints
app.MapHealthChecks("/health");

app.MapControllers();

app.Run();
