using Microsoft.AspNet.Hosting;

namespace HelloWeb
{
    public class Program
    {
		public static void Main(string[] args)
		{
            var configuration = WebApplicationConfiguration.GetDefault(args);
            
            var application = new WebApplicationBuilder()
                        .UseConfiguration(configuration)
                        .UseStartup<Startup>()
                        .Build();

            application.Run();
		}	
	}
}