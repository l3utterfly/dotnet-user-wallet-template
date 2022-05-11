using System.Net;

namespace WebApi.Exceptions
{
    /// <summary>
    /// Base exception for use to returned handled http status codes, this exception is tested against in the custom http exception middleware
    /// </summary>
    [Serializable]
    public class HandledException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public HttpStatusCode StatusCode { get; }

        public HandledException(HttpStatusCode httpStatusCode, string message) : base(message)
        {
            StatusCode = httpStatusCode;
        }
    }
}
