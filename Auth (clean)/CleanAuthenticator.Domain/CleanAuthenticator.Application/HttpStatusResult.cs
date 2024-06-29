using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CleanAuthenticator.Application
{
    public class HttpStatusResult
    {
        public HttpStatusCode StatusCode { get; set; }
        public object Message { get; set; }
    }
}
