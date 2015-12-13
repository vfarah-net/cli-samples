using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace Microsoft.AspNet.Hosting
{
    public static class WebHost
    {
        public static void ExecuteInChildAppDomain(string[] args)
        {
            var domain = CreateDomain(args);

            if (domain != null)
            {
                int exitCode = domain.ExecuteAssemblyByName(Assembly.GetEntryAssembly().FullName, args);
                Environment.Exit(exitCode);
            }
        }
        
        public static void ExecuteCallbackInChildAppDomain(string[] args, CrossAppDomainDelegate callback)
        {
            var domain = CreateDomain(args);

            if (domain != null)
            {
                domain.DoCallBack(callback);
            }
        }
        
        private static AppDomain CreateDomain(string[] args)
        {
            if (AppDomain.CurrentDomain.IsDefaultAppDomain())
            {
                var config = new ConfigurationBuilder()
                                    .AddCommandLine(args)
                                    .Build();

                var applicationBasePath = config["appbase"] ?? Environment.CurrentDirectory;

                // If the applicationBasePath doesn't match the default domain then create a new domain

                var setup = new AppDomainSetup
                {
                    // Specify the new appbase
                    ApplicationBase = applicationBasePath,

                    // Path to where the assemblies are
                    PrivateBinPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)
                };

                // Create a child domain and execute main again
                return AppDomain.CreateDomain("webdomain", AppDomain.CurrentDomain.Evidence, setup);
            }

            return null;
        }
    }
}