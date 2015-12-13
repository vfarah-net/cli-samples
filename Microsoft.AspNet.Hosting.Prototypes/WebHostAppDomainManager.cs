using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace Microsoft.AspNet.Hosting
{
    public class WebHostAppDomainManager : AppDomainManager
    {
		public override void InitializeNewDomain(AppDomainSetup appDomainInfo)
        {
            var config = new ConfigurationBuilder()
                                    .AddCommandLine(Environment.GetCommandLineArgs())
                                    .Build();

            var applicationBasePath = config["appbase"] ?? Environment.CurrentDirectory;

            // Specify the new appbase
            appDomainInfo.ApplicationBase = applicationBasePath;

            // Path to where the assemblies are
            appDomainInfo.PrivateBinPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            base.InitializeNewDomain(appDomainInfo);
        }
	}
}