namespace CcbStack.Core.Runtime;

/// <summary>An executable located on the current <c>PATH</c>.</summary>
/// <param name="Name">The executable name that was searched for (e.g. <c>"git"</c>).</param>
/// <param name="FullPath">The full path to the resolved executable.</param>
public sealed record ResolvedExecutable(string Name, string FullPath);
