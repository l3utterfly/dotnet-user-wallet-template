namespace WebApi.Helpers;

using System.Globalization;
using System.Net;

// custom exception class for throwing application specific exceptions (e.g. for validation) 
// that can be caught and handled within the application
public class HandledException : Exception
{
    private readonly HttpStatusCode _statusCode;
    public HttpStatusCode StatusCode { get { return _statusCode; } }

    public HandledException() : base() {}

    public HandledException(HttpStatusCode httpStatusCode, string message) : base(message)
    {
        _statusCode = httpStatusCode;
    }

    
}