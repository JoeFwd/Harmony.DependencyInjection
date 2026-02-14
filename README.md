# Harmony.DependencyInjection

## Overview

`Harmony.DependencyInjection` provides a lightweight way to integrate **Harmony** patches into a .NET application using
the dependency‑injection pattern. It discovers classes that implement the `IPatch` interface, registers the required
services, and applies the patches automatically when the host starts.

## Creating a Service Class (Dependency)

```csharp
public interface IMyService
{
    void DoWork();
}

public class MyService : IMyService
{
    public void DoWork() => Console.WriteLine("Service work executed.");
}
```

## Creating a Patch (`IPatch` implementation) with a Dependency

```csharp
using System.Reflection;
using Harmony.DependencyInjection.Patches;

public class MyPatch : IPatch
{
    // Dependency injected by the DI container
    private static IMyService _service;

    // Constructor receives the service
    public MyPatch(IMyService service)
    {
        _service = service;
    }

    // The method that will be patched
    public MethodInfo? TargetMethod => typeof(MyTargetClass).GetMethod("TargetMethod", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
    
    // The method that contains the patch logic
    public MethodInfo? PatchMethod => typeof(MyPatch).GetMethod(nameof(Prefix), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

    // The type of patch (prefix, postfix, transpiler)
    public PatchType PatchType => PatchType.Prefix;

    // Prefix method – can use the injected service
    public static bool Prefix()
    {
        // Use the injected service before the original method runs
        _service.DoWork();
        return true; // continue with the original method
    }
}
```

## Registering Services, the Patch, and Initialising the Harmony Patcher

When configuring the DI container, **register the dependency, the patch, and then the Harmony services**. The hosted
service will apply the patch once the host is built and started.

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Harmony.DependencyInjection;

IServiceCollection serviceCollection = new ServiceCollection();

// 1. Register the service that the patch depends on
serviceCollection.AddSingleton<IMyService, MyService>();

// 2. Register the patch implementation so it can be discovered (DI will inject IMyService)
serviceCollection.AddSingleton<IPatch, MyPatch>();

// 3. Add loggers
serviceCollection.AddLogging();

// 4. Register Harmony services and the hosted patcher
serviceCollection.AddHarmonyPatching();

// Build all services
IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

// Apply all Harmony  patches
serviceProvider.GetService<IHarmonyPatcher>().ApplyPatches();
```

## Patch Applied Confirmation

When the service provider is being built, the `HarmonyPatcher` discovers `MyPatch` resolves its
dependencies. Only when `ApplyPatches` is called, is the patch applied via Harmony. You can verify the patch was applied
by checking the logs (the service logs a confirmation
when patches are applied) or by observing the altered behaviour of `MyTargetClass.TargetMethod` (the service's `DoWork`
method will be executed before the original method).

## Running the Tests

```bash
 dotnet test Harmony.DependencyInjection.Tests/Harmony.DependencyInjection.Tests.csproj
```

## Contributing

1. Fork the repository and clone it.
2. Create a feature branch (`git checkout -b feature/your-feature`).
3. Implement your changes and add unit tests.
4. Ensure the full test suite passes.
5. Open a pull request with a clear description.

---

*Happy patching!*