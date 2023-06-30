using System.Net;
namespace ChatApp.Services;

public class ApiException : Exception
{
    public HttpStatusCode Code;

    public ApiException()
    {
    }

    public ApiException(string message) : base(message)
    {
    }

    public ApiException(string message, HttpStatusCode code) : base(message)
    {
        this.Code = code;
    }
}