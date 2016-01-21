using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.HttpOverrides;

namespace HelloWeb
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseIISPlatformHandler();
            app.UseOverrideHeaders(new OverrideHeaderOptions
            {
                ForwardedOptions = ForwardedHeaders.All
            });
            
            app.Run(context =>
            {
                return context.Response.WriteAsync("Hello World!");
            });
        }
    }
}