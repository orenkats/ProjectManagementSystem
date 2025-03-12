namespace ProjectManagementSystem.Common.Exceptions;

public class BaseException : Exception
{
    public int HttpStatusCode { get; }

    public BaseException(string message, int httpStatusCode = 500) : base(message)
    {
        HttpStatusCode = httpStatusCode; 
    }
}