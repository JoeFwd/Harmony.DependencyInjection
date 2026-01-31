namespace Harmony.DependencyInjection.Patches;

/// <summary>
/// Specifies the type of Harmony patch to apply.
/// See the Harmony documentation for details on each patch type:
/// <see href="https://harmony.pardeike.net/articles/patching.html#transpilers"/> for Transpiler, 
/// <see href="https://harmony.pardeike.net/articles/patching.html#prefix"/> for Prefix, and 
/// <see href="https://harmony.pardeike.net/articles/patching.html#postfix"/> for Postfix.
/// </summary>
public enum PatchType
{
    /// <summary>
    /// A transpiler modifies the target method's IL code.
    /// </summary>
    Transpiler,
    /// <summary>
    /// A prefix runs before the original method.
    /// </summary>
    Prefix,
    /// <summary>
    /// A postfix runs after the original method.
    /// </summary>
    Postfix
}
