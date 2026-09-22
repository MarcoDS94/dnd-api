namespace Dnd.Api.Middleware;

public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(() =>
        {
            var headers = context.Response.Headers;

            // Prevent MIME type sniffing
            headers.TryAdd("X-Content-Type-Options", "nosniff");

            // Prevent clickjacking by denying framing
            headers.TryAdd("X-Frame-Options", "DENY");

            // Control referrer information sent in HTTP requests
            headers.TryAdd("Referrer-Policy", "strict-origin-when-cross-origin");

            // Restrict browser features and APIs (camera, microphone, geolocation, etc.)
            headers.TryAdd("Permissions-Policy", "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()");

            // Content Security Policy
            var path = context.Request.Path.Value ?? string.Empty;
            if (!path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase))
            {
                headers.TryAdd("Content-Security-Policy", "default-src 'none'; frame-ancestors 'none';");
            }
            else
            {
                headers.TryAdd("Content-Security-Policy", "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; img-src 'self' data:;");
            }

            return Task.CompletedTask;
        });

        await _next(context);
    }
}

