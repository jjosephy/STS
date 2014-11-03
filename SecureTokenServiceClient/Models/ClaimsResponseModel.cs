using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SecureTokenServiceClient.Models
{
    public class ClaimsResponseModel
    {
        [JsonProperty(PropertyName = "authentication")]
        public string Authentication
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "userName")]
        public string UserName
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "userId")]
        public string UserId
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

        [JsonProperty(PropertyName = "mobilePhone")]
        public string MobilePhone
        {
            get;
            set;
        }
    }
}