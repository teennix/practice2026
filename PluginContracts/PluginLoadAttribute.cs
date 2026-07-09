using System;

namespace PluginContracts;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class PluginLoadAttribute : Attribute { }