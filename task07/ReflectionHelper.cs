using System;
using System.Reflection;
using System.Linq;

namespace task07;

public static class ReflectionHelper
{
    public static void PrintTypeInfo(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var classDisplay = type.GetCustomAttribute<DisplayNameAttribute>();
        if (classDisplay != null)
        {
            Console.WriteLine($"Класс: {classDisplay.DisplayName}");
        }

        var classVersion = type.GetCustomAttribute<VersionAttribute>();
        if (classVersion != null)
        {
            Console.WriteLine($"Версия: {classVersion.Major}.{classVersion.Minor}");
        }

        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
            .Select(p => new { p.Name, Attr = p.GetCustomAttribute<DisplayNameAttribute>() })
            .Where(p => p.Attr != null)
            .ToList();

        if (properties.Count > 0)
        {
            Console.WriteLine("Свойства:");
            properties.ForEach(p => Console.WriteLine($"  - {p.Name}: {p.Attr!.DisplayName}"));
        }

        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
            .Select(m => new { m.Name, Attr = m.GetCustomAttribute<DisplayNameAttribute>() })
            .Where(m => m.Attr != null)
            .ToList();

        if (methods.Count > 0)
        {
            Console.WriteLine("Методы:");
            methods.ForEach(m => Console.WriteLine($"  - {m.Name}: {m.Attr!.DisplayName}"));
        }
    }
}