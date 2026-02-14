using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Harmony.DependencyInjection.Patches;
using Harmony.DependencyInjection.Tests.Integration.Patches;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Harmony.DependencyInjection.Tests.Integration.Util;

public static class RoslynCompilationService
{
    public static Assembly Compile(string assemblyName, string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        // 1. Define the Critical Assemblies
        // We use a HashSet to ensure we don't add the same assembly twice.
        HashSet<string> referencePaths = new(StringComparer.OrdinalIgnoreCase);

        Type[] typesToreference = new[]
        {
            typeof(object),
            typeof(IPatch),
            typeof(IServiceCollection),
            typeof(ServiceCollection),
            typeof(DummyPatch),
            typeof(HarmonyPatcherRegistration),
            typeof(IHarmonyPatcher),
            typeof(LoggerFactory) // Added reference for logging assembly
        };

        // 2. Add Critical Assemblies
        foreach (var type in typesToreference)
            if (!string.IsNullOrEmpty(type.Assembly.Location))
                referencePaths.Add(type.Assembly.Location);

        // 3. Add Trusted Platform Assemblies (Runtime Core)
        // This ensures System.dll, netstandard.dll etc are found correctly on .NET Core/Standard
        var trustedAssemblies = (string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES");
        if (!string.IsNullOrEmpty(trustedAssemblies))
            foreach (var path in trustedAssemblies.Split(Path.PathSeparator))
            {
                var fileName = Path.GetFileName(path);
                if (fileName.StartsWith("System.") ||
                    fileName.StartsWith("mscorlib") ||
                    fileName.StartsWith("netstandard"))
                    referencePaths.Add(path);
            }

        // 4. Create Metadata References
        List<PortableExecutableReference> references = referencePaths
            .Select(path => MetadataReference.CreateFromFile(path))
            .ToList();

        var compilation = CSharpCompilation.Create(
            assemblyName,
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var ms = new MemoryStream();
        var result = compilation.Emit(ms);

        if (!result.Success)
        {
            // Format errors clearly
            var errors = string.Join(Environment.NewLine, result.Diagnostics
                .Where(d => d.Severity == DiagnosticSeverity.Error)
                .Select(d => $"Line {d.Location.GetLineSpan().StartLinePosition.Line + 1}: {d.GetMessage()}"));
            throw new InvalidOperationException($"Compilation Failed:{Environment.NewLine}{errors}");
        }

        ms.Seek(0, SeekOrigin.Begin);
        return Assembly.Load(ms.ToArray());
    }
}