using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;

namespace DynamicReloader
{
    public class DynamicLoadContext : AssemblyLoadContext
    {
        private readonly Dictionary<AssemblyName, string> _assemblies;
        private readonly Dictionary<string, string> _dllImports;
        private readonly Dictionary<string, ProjectId> _projects;

        private readonly Dictionary<AssemblyName, Assembly> _assemblyCache = new Dictionary<AssemblyName, Assembly>();

        private readonly Workspace _workspace;

        public DynamicLoadContext(Workspace workspace, Dictionary<string, ProjectId> projects, Dictionary<AssemblyName, string> assemblies, Dictionary<string, string> dllImports)
        {
            _workspace = workspace;
            _projects = projects;
            _assemblies = assemblies;
            _dllImports = dllImports;
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            System.Console.WriteLine($"Load({assemblyName})");
            ProjectId projectId;
            if (_projects.TryGetValue(assemblyName.Name, out projectId))
            {
                var project = _workspace.CurrentSolution.GetProject(projectId);
                System.Console.WriteLine($"Found project {project.AssemblyName}({(project.Id)})");

                using (var assemblySymbols = new MemoryStream())
                using (var assemblyStream = new MemoryStream())
                {
                    var emitOptions = new EmitOptions().WithDebugInformationFormat(DebugInformationFormat.PortablePdb);
                    var compilation = project.GetCompilationAsync().GetAwaiter().GetResult();
                    var emitResult = compilation.Emit(assemblyStream, assemblySymbols, options: emitOptions);

                    if (emitResult.Success)
                    {
                        assemblyStream.Position = 0;
                        assemblySymbols.Position = 0;

                        return LoadFromStream(assemblyStream, assemblySymbols);
                    }
                }
            }

            string path;
            if (_assemblies.TryGetValue(assemblyName, out path))
            {
                System.Console.WriteLine($"LoadFromAssemblyPath({path})");
                return LoadFromAssemblyPath(path);
            }

            return null;
        }
        
        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            System.Console.WriteLine($"LoadUnmanagedDll({unmanagedDllName})");
            return base.LoadUnmanagedDll(unmanagedDllName);
        }
    }

}