using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SecureTokenServiceClient.Models
{
    public class AuthenticationModel
    {
        [JsonProperty(PropertyName = "accessToken")]
        public string AccessToken
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "email")]
        public string Email
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "extensionId")]
        public string ExtenstionId
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "mobilephone")]
        public string MobilePhone
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "password")]
        public string Password
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "phone")]
        public string Phone
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "redirectUrl")]
        public string RedirectUrl
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "username")]
        public string UserName
        {
            get;
            set;
        }
    }
}