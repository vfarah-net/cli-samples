using System.IO;
using Microsoft.AspNet.Hosting;

namespace HelloWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {            
            var host = new WebHostBuilder()
                        .UseDefaultConfiguration(args)
                        .UseServer("Microsoft.AspNet.Server.Kestrel")
                        .UseApplicationBasePath(Directory.GetCurrentDirectory())
                        .UseStartup<Startup>()
                        .Build();

            host.Run();
        }    
    }
}