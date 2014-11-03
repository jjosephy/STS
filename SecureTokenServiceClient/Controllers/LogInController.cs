using Newtonsoft.Json;
using SecureTokenServiceClient.Client;
using SecureTokenServiceClient.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SecureTokenServiceClient.Controllers
{
    public class LogInController : Controller
    {
        const string RelyingParty = "http://custom.content.net";
        static Guid AppId = new Guid("8A8B6062-2657-43D4-83A3-9A6BEB8E1EF4");

        // GET: LogIn
        public async Task<string> Index(string email, string password, string username, string mobile)
        {
            try
            {
                var body = new AuthenticationModel
                {
                    AccessToken = AppId.ToString(),
                    Email = email,
                    Password = password,
                    UserName = username,
                    MobilePhone = mobile
                };

                var tokenClient = new TokenClient();
                var tokenResponse = await tokenClient.GetTokenAsync(body);

                if (tokenResponse.StatusCode == HttpStatusCode.OK)
                {
                      var claims = await tokenClient.GetClaimsAsync(tokenResponse.AuthToken);
                    return tokenResponse.AuthToken + "<br/>" + JsonConvert.SerializeObject(claims); 
                }

                Response.StatusCode = 401;
                Response.Redirect("/?l=f");
                return string.Empty;
            }
            catch(Exception ex)
            {
                return "An exception occured" + ex.Message;
            }
        }
    }
}