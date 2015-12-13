using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;

namespace HelloWebFullAppDomainManagerPrototype
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.Run(context =>
            {
                return context.Response.WriteAsync($"Hello World!");
            });
        }
    }
}