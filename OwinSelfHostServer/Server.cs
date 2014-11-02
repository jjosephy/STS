using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OwinSelfHostServer
{
    public class Server<T>
    {
        private Server()
        { }

        public static Server<T> Create(T type)
        {
            string baseAddress = "http://localhost:9000/"; 

            // Start OWIN host 

            StartOptions op = new StartOptions();

            WebApp.Start<T>(url: baseAddress);

            return null;
        }

    }
}
