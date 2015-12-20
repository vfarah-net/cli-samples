using Microsoft.AspNet.Hosting;

namespace HelloMvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var application = new WebApplicationBuilder()
                        .UseConfiguration(WebApplicationConfiguration.GetDefault(args))
                        .UseStartup<Startup>()
                        .Build();

            application.Run();
        }
    }
}