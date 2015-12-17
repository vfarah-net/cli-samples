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
                }

                foreach (var asset in export.NativeLibraries)
                {
                    dllImports[asset.Name] = asset.ResolvedPath;
                }
            }

            var loadContext = new DynamicLoadContext(workspace, projects, dllImports);

            // We need to force load all assemblies in the load context since we don't want
            // things to transitively fall back to the default load context
            foreach (var asm in assemblies)
            {
                try
                {
                    // Force load the assemblies
                    loadContext.LoadFile(asm.Value);
                }
                catch
                {
                    // It's in the TPA list
                    Console.WriteLine($"Failed to load assembly {asm.Key}. It's in the TPA list");
                }
            }

            var assembly = loadContext.LoadFromAssemblyName(new AssemblyName(compileTimeContext.ProjectFile.Name));

            if (assembly.EntryPoint != null)
            {
                assembly.EntryPoint.Invoke(null, new object[] { args.Skip(1).ToArray() });
            }
        }
    }
}