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
        private readonly Dictionary<string, string> _dllImports;
        private readonly Dictionary<string, ProjectId> _projects;

        private readonly Dictionary<string, Assembly> _assemblies = new Dictionary<string, Assembly>();

        private readonly Workspace _workspace;

        public DynamicLoadContext(Workspace workspace, 
                                  Dictionary<string, ProjectId> projects, 
                                  Dictionary<string, string> dllImports)
        {
            _workspace = workspace;
            _projects = projects;
            _dllImports = dllImports;
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            Assembly assembly;
            if (_assemblies.TryGetValue(assemblyName.Name, out assembly))
            {
                return assembly;
            }
            
            try
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
            }
            catch
            {
                return Default.LoadFromAssemblyName(assemblyName);
            }
            
            return null;
        }
        
        public Assembly LoadFile(string assemblyPath)
        {
            System.Console.WriteLine($"LoadFile({assemblyPath})");
            var assembly = LoadFromAssemblyPath(assemblyPath);
            if (assembly != null)
            {
                _assemblies[assembly.GetName().Name] = assembly;
            }
            return assembly;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            System.Console.WriteLine($"LoadUnmanagedDll({unmanagedDllName})");
            string path;
            if(_dllImports.TryGetValue(unmanagedDllName, out path))
            {
                return LoadUnmanagedDllFromPath(path);
            }
            return base.LoadUnmanagedDll(unmanagedDllName);
        }
    }

}