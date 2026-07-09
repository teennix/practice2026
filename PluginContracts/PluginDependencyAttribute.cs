using System;

namespace PluginContracts;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class PluginDependencyAttribute : Attribute
{
    public string DependencyName { get; }

    public PluginDependencyAttribute(string dependencyName)
    {
        ArgumentNullException.ThrowIfNull(dependencyName);
        DependencyName = dependencyName;
    }
}