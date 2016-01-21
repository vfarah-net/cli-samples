using System.IO;
using Microsoft.AspNet.Hosting;

namespace HelloMvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                        .UseServer("Microsoft.AspNet.Server.Kestrel")
                        .UseApplicationBasePath(Directory.GetCurrentDirectory())
                        .UseDefaultConfiguration(args)
                        .UseIISPlatformHandlerUrl()
                        .UseStartup<Startup>()
                        .Build();

            host.Run();
        }
    }
}