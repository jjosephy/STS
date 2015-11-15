using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace JJ.SecureTokenService.HttpResponseMessages
{
    /// <summary>
    /// Wrapper methods to easily return messages. Each one of these messages should log
    /// </summary>
    public class ServiceResponseMessage
    {
        public static HttpResponseMessage Unauthorized()
        {
            return CreateContent(HttpStatusCode.Unauthorized, "Unathorized");
        } 

        public static HttpResponseMessage Ok(object value)
        {
            return CreateContent(HttpStatusCode.OK, JsonConvert.SerializeObject(value));
        }

        public static HttpResponseMessage BadRequest(string message)
        {
            return CreateContent(HttpStatusCode.BadRequest, message);
        }

        public static HttpResponseMessage InternalServerError(string message)
        {
            return CreateContent(HttpStatusCode.InternalServerError, message);
        }

        private static HttpResponseMessage CreateContent(HttpStatusCode code, string message)
        {
            return new HttpResponseMessage(code)
            {
                Content = new StringContent(message)
            };
        }
    }
}