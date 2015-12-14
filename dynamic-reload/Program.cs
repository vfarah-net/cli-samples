using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.DotNet.ProjectModel;
using Microsoft.DotNet.ProjectModel.Workspaces;

namespace DynamicReloader
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var directory = args.Length == 0 ? Directory.GetCurrentDirectory() : Path.GetFullPath(args[0]);

            // Pick the corret runtime id (based on something)
            // Compile time context
            var compileTimeContext = ProjectContext.CreateContextForEachFramework(directory).First();
            Console.WriteLine($"TFM: {compileTimeContext.TargetFramework}");
            var workspace = compileTimeContext.CreateWorkspace();
            var projects = new Dictionary<string, ProjectId>();

            foreach (var project in workspace.CurrentSolution.Projects)
            {
                // Projects have the name {name+tfm}
                projects[project.AssemblyName] = project.Id;
            }

            // Runtime context
            var runtimeContext = ProjectContext.CreateContextForEachTarget(directory).Last();
            Console.WriteLine($"TFM + RID: {runtimeContext.TargetFramework} {runtimeContext.RuntimeIdentifier}");
            var exporter = runtimeContext.CreateExporter("Debug");

            var assemblies = new Dictionary<AssemblyName, string>(AssemblyNameComparer.OrdinalIgnoreCase);
            var dllImports = new Dictionary<string, string>();

            var dotnetHome = Environment.GetEnvironmentVariable("DOTNET_HOME");
            Console.WriteLine($"DOTNET_HOME: {dotnetHome}");

            var runtimePath = Path.Combine(dotnetHome, "runtime", "coreclr");

            foreach (var export in exporter.GetAllExports())
            {
                if (export.Library is ProjectDescription)
                {
                    continue;
                }

                // TODO: Handle resource assemblies
                foreach (var asset in export.RuntimeAssemblies)
                {
                    var assemblyName = new AssemblyName(asset.Name);
                    assemblies[assemblyName] = asset.ResolvedPath;

                    var runtimeDllPath = Path.Combine(runtimePath, asset.Name + ".dll");

                    if (File.Exists(runtimeDllPath))
                    {
                        assemblies[assemblyName] = runtimeDllPath;
                    }
                }

                foreach (var asset in export.NativeLibraries)
                {
                    dllImports[asset.Name] = asset.ResolvedPath;
                }
            }

            var loadContext = new DynamicLoadContext(workspace, projects, assemblies, dllImports);

            var assembly = loadContext.LoadFromAssemblyName(new AssemblyName("System.Runtime"));

            Console.WriteLine(assembly);
        }
    }
}