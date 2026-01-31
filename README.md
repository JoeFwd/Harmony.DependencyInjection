# Harmony.DependencyInjection

## Overview

`Harmony.DependencyInjection` provides a lightweight way to integrate **Harmony** patches into a .NET application using the dependency‑injection pattern. It discovers classes that implement the `IPatch` interface, registers the required services, and applies the patches automatically when the host starts.

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
using HarmonyLib;
using Harmony.DependencyInjection.Patches;

[HarmonyPatch(typeof(MyTargetClass))]
public class MyPatch : IPatch
{
    // Dependency injected by the DI container
    private readonly IMyService _service;

    // Constructor receives the service
    public MyPatch(IMyService service)
    {
        _service = service;
    }

    // The method that will be patched
    public MethodInfo? TargetMethod => AccessTools.Method(typeof(MyTargetClass), "TargetMethod");

    // The method that contains the patch logic
    public MethodInfo? PatchMethod => AccessTools.Method(typeof(MyPatch), nameof(Prefix));

    // The type of patch (prefix, postfix, transpiler)
    public PatchType PatchType => PatchType.Prefix;

    // Prefix method – can use the injected service
    public bool Prefix()
    {
        // Use the injected service before the original method runs
        _service.DoWork();
        return true; // continue with the original method
    }
}
```

## Registering Services, the Patch, and Initialising the Harmony Patcher

When configuring the DI container, **register the dependency, the patch, and then the Harmony services**. The hosted service will apply the patch once the host is built and started.

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Harmony.DependencyInjection;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // 1. Register the service that the patch depends on
        services.AddSingleton<IMyService, MyService>();

        // 2. Register the patch implementation so it can be discovered (DI will inject IMyService)
        services.AddTransient<MyPatch>();

        // 3. Register Harmony services and the hosted patcher
        services.AddHarmonyPatching();
    })
    .Build();

await host.StartAsync(); // Host (DI container) is built and started here
```

## Patch Applied Confirmation

After the host has been started, the `HarmonyPatcher` hosted service runs, discovers `MyPatch`, resolves its dependencies, and applies it. You can verify the patch was applied by checking the logs (the service logs a confirmation when patches are applied) or by observing the altered behaviour of `MyTargetClass.TargetMethod` (the service's `DoWork` method will be executed before the original method).

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