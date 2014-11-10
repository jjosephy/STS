using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
            return new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                Content = new StringContent("Unathorized")
            };
        } 

        public static HttpResponseMessage Ok(object value)
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(value))
            };
        }

        public static HttpResponseMessage BadRequest(string message)
        {
            return new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(message)
            };
        }
    }
}