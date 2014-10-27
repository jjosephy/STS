
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace OwinSelfHostTestServer
{
    /// <summary>
    /// Provides the configuration to Self Host Controllers for Testing
    /// </summary>
    public class OwinTestConfiguration
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Services.Replace(typeof(IAssembliesResolver), new TestAssemblyResolver());
            config.MapHttpAttributeRoutes();
            app.UseWebApi(config);
        }
    }
}
