using System.Net;

namespace Authenticator.Extensions
{
    public class HttpStatusResult
    {
        public HttpStatusCode StatusCode { get; set; }
        public object Message { get; set; } 
    }
}
