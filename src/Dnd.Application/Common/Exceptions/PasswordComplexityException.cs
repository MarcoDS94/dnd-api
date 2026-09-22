namespace Dnd.Application.Common.Exceptions;

public class PasswordComplexityException : Exception
{
    public PasswordComplexityException(string message) : base(message)
    {
    }
}

