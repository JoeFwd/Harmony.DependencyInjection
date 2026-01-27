using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace Harmony.DependencyInjection.Tests
{
    /// <summary>
    /// Helper for compiling C# source code at runtime using Roslyn.
    /// </summary>
    public static class DynamicAssemblyHelper
    {
        /// <summary>
        /// Compiles the given C# source code into an in‑memory assembly.
        /// </summary>
        /// <param name="code">C# source code to compile.</param>
        /// <param name="references">Optional additional metadata references. If null, all currently loaded non‑dynamic assemblies are used.</param>
        /// <returns>The compiled <see cref="Assembly"/>.</returns>
        public static Assembly CompileAssembly(string code)
        {
            if (code == null) throw new ArgumentNullException(nameof(code));

            // Parse the source code into a syntax tree.
            var syntaxTree = CSharpSyntaxTree.ParseText(code);

            // Determine metadata references.
            var metadataReferences = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
                .Select(a => (MetadataReference)MetadataReference.CreateFromFile(a.Location))
                .ToList();

            // Create the compilation.
            var compilation = CSharpCompilation.Create(
                assemblyName: $"DynamicAssembly_{Guid.NewGuid()}",
                syntaxTrees: new[] { syntaxTree },
                references: metadataReferences,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            // Emit the assembly to a memory stream.
            using var ms = new MemoryStream();
            var result = compilation.Emit(ms);

            if (!result.Success)
            {
                // Gather diagnostic messages for easier debugging.
                var failures = result.Diagnostics
                    .Where(d => d.IsWarningAsError || d.Severity == DiagnosticSeverity.Error)
                    .Select(d => d.ToString());
                throw new InvalidOperationException($"Compilation failed: {string.Join(Environment.NewLine, failures)}");
            }

            ms.Seek(0, SeekOrigin.Begin);
            return Assembly.Load(ms.ToArray());
        }

        /// <summary>
        /// Retrieves a type from the given assembly by its full name.
        /// </summary>
        /// <param name="assembly">The assembly to search.</param>
        /// <param name="fullName">Full name of the type (namespace + class name).</param>
        /// <returns>The <see cref="Type"/> if found; otherwise throws.</returns>
        public static Type FindType(Assembly assembly, string fullName)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            if (string.IsNullOrWhiteSpace(fullName)) throw new ArgumentException("Type name must be provided", nameof(fullName));
            var type = assembly.GetType(fullName, throwOnError: false, ignoreCase: false);
            if (type == null)
                throw new TypeLoadException($"Type '{fullName}' not found in assembly '{assembly.FullName}'.");
            return type;
        }
    }
}
