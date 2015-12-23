using System.Reflection;
using Microsoft.AspNet.Mvc.Infrastructure;
using Microsoft.AspNet.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.CompilationAbstractions;

namespace HelloMvc
{
    public static class MvcServiceCollectionExtensions
    {
        /// Override the default AddMvc since we need to do some fix up for the CLI
        // This is temporary as more things come online
        public static IMvcBuilder AddMvc2(this IServiceCollection services)
        {
            // We need to disable register an empty exporter so that we can add references
            // https://github.com/aspnet/Mvc/issues/3633
            services.AddSingleton<ILibraryExporter, NullExporter>();

            // ILibraryManager isn't available yet so we need to explicitly add the assemblies
            // that we want to find controllers in.
            
            // This will be replaced with the dependency context. 
            var assemblyProvider = new StaticAssemblyProvider();
            assemblyProvider.CandidateAssemblies.Add(typeof(Startup).GetTypeInfo().Assembly);
            services.AddSingleton<IAssemblyProvider>(assemblyProvider);
            
            // Override the options and add references
            services.AddScoped<IConfigureOptions<RazorViewEngineOptions>, MvcCompilationOptions>();
            
            return services.AddMvc();
        }
    }
}