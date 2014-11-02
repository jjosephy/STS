using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using Microsoft.Owin.Security.OAuth;
using JJ.SecureTokenService.Handler;

namespace JJ.SecureTokenService
{
    public partial class OwinStartUp
    {
        public static OAuthAuthorizationServerOptions OAuthBearerOptions { get; private set; }
        public static OAuthBearerAuthenticationOptions OAuthBearerAuthenticationOptions { get; private set; }

        public void ConfigureAuth(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.MessageHandlers.Add(new CertificateHandler());
            
            OAuthBearerOptions = new OAuthAuthorizationServerOptions();
            app.UseOAuthAuthorizationServer(OAuthBearerOptions);

            OAuthBearerAuthenticationOptions = new OAuthBearerAuthenticationOptions();
            app.UseOAuthBearerAuthentication(OAuthBearerAuthenticationOptions);

            bool loadAssemblies = false;
            if (bool.TryParse(ConfigurationManager.AppSettings["LoadAssemblyForTest"], out loadAssemblies))
            {
                config.Services.Replace(typeof(IAssembliesResolver), new AssemblyResolver());
            }

            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter("Bearer"));
            
            app.UseWebApi(config);
        }
    }
}