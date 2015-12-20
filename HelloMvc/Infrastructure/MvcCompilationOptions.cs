

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNet.Mvc.Razor;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.OptionsModel;
using Microsoft.Extensions.PlatformAbstractions;

namespace HelloMvc
{
    public class MvcCompilationOptions : ConfigureOptions<RazorViewEngineOptions>
    {
        public MvcCompilationOptions(IApplicationEnvironment applicationEnvironment) : base(options => Configure(options, applicationEnvironment))
        {

        }

        private static void Configure(RazorViewEngineOptions options, IApplicationEnvironment appEnvironment)
        {
            var previous = options.CompilationCallback;

            options.CompilationCallback = c =>
            {
                // Compose with the previous options
                previous(c);
                
                System.Console.WriteLine();
                
                var refs = ResolveCompilationReferences(appEnvironment);

                c.Compilation = c.Compilation
                    .AddReferences(refs);
            };
        }
        
        private static IEnumerable<MetadataReference> ResolveCompilationReferences(IApplicationEnvironment appEnvironment)
        {
            // See the following issues for progress on resolving references:
            // https://github.com/aspnet/Mvc/issues/3633
            // https://github.com/dotnet/cli/issues/376
            
            // HACK WARNING: We don't have a way to get the reference assemblies at runtime
            // so this is super hacky and parses the response file passed into dotnet-compile-csc in order to discover 
            // compile time references. It's VERY fragile and barely works :).
            var responseFileName = $"dotnet-compile.{appEnvironment.ApplicationName}.rsp";
            var baseDir = new DirectoryInfo(appEnvironment.ApplicationBasePath);
            string responseFilePath = Path.Combine(baseDir.FullName, responseFileName);

            if (!File.Exists(responseFilePath))
            {
                responseFilePath = Path.Combine(baseDir.Parent.Parent.Parent.FullName, "obj", baseDir.Parent.Name, baseDir.Name, responseFileName);
            }

            if (!File.Exists(responseFilePath))
            {
                // Logic was too flaky
                throw new InvalidOperationException("Unable to resolve references!");
            }

            // Parse line by line and find the references
            var refs = File.ReadAllLines(responseFilePath)
                           .Where(l => l.StartsWith("--reference:"))
                           .Select(l => l.Substring("--reference:".Length))
                           .Select(path => MetadataReference.CreateFromFile(path));
            return refs;
        }
    }
}