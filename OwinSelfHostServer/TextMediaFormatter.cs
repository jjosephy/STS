using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace OwinSelfHostServer
{
    public class TextMediaFormatter : MediaTypeFormatter
    {
        public TextMediaFormatter()
        {
            this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/plain"));
        }

        public override bool CanReadType(Type type)
        {
            return type == typeof(string);
        }

        public override bool CanWriteType(Type type)
        {
            return type == typeof(string);
        }

        public override Task WriteToStreamAsync(
            Type type, 
            object value, 
            Stream writeStream, 
            HttpContent content, 
            TransportContext transportContext)
        {
            return Task.Run(async () =>
            {
                var writer = new StreamWriter(writeStream);
                await writer.WriteAsync(value.ToString());
                await writer.FlushAsync();
            });
        }
    }
}