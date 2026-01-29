using System;
using System.Reflection;

namespace Harmony.DependencyInjection.Tests.Integration.Util;

public static class AssemblyLoader
{
    public static object CreateInstance(Assembly assembly, string typeName)
    {
        var type = assembly.GetType(typeName)
                   ?? throw new TypeLoadException(typeName);

        return Activator.CreateInstance(type)!;
    }
}