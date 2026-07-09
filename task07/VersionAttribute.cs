using System;

namespace task07;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class VersionAttribute : Attribute
{
    public int Major { get; }
    public int Minor { get; }

    public VersionAttribute(int major, int minor)
    {
        Major = major;
        Minor = minor;
    }
}