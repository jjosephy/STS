
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace JJ.SecureTokenService.Contracts.V1
{
    public class TokenRequestV1 : TokenRequestBase
    {
        [JsonProperty(PropertyName = "authProvider")]
        public string AuthenticationProvider
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