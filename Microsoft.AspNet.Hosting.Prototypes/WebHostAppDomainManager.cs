using System;
using System.Linq;
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
                                    .AddCommandLine(Environment.GetCommandLineArgs().Skip(1).ToArray())
                                    .Build();

            var applicationBasePath = config["appbase"] ?? Environment.CurrentDirectory;

            // Specify the new appbase
            appDomainInfo.ApplicationBase = applicationBasePath;

            // Path to where the assemblies are
            appDomainInfo.PrivateBinPath = Path.GetDirectoryName(typeof(WebHostAppDomainManager).Assembly.Location);

            base.InitializeNewDomain(appDomainInfo);
        }
    }
}