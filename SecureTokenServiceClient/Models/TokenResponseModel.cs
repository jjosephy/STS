using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace SecureTokenServiceClient.Models
{
    public class TokenResponseModel
    {
        public string AuthToken
        {
            get;
            set;
        }

        public HttpStatusCode StatusCode
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }
    }
}