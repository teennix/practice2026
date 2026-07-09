using System;

namespace task07;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class DisplayNameAttribute : Attribute
{
    public string DisplayName { get; }

    public DisplayNameAttribute(string displayName)
    {
        ArgumentNullException.ThrowIfNull(displayName);
        DisplayName = displayName;
    }
}