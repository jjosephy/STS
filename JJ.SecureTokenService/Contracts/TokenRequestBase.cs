using JJ.SecureTokenService.Authentication;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JJ.SecureTokenService.Contracts
{
    public class TokenRequestBase
    {
        [JsonProperty(PropertyName = "extensionId")]
        public string ExtenstionId
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "accessToken")]
        public string AccessToken
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "authenticationProvider")]
        public AuthenticationProvider AuthenticationProvider
        {
            get;
            set;
        }
    }
}