using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JJ.SecureTokenService.Contracts.V1
{
    public class TokenResponseV1
    {
        [JsonProperty(PropertyName = "token")]
        public string Token
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "level")]
        public string AuthLevel
        {
            get;
            set;
        }
    }
}