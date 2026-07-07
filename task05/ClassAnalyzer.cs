using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace task05;

public class ClassAnalyzer
{
    private readonly Type _type;

    public ClassAnalyzer(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        _type = type;
    }

    public IEnumerable<string> GetPublicMethods()
    {
        return _type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                    .Select(m => m.Name);
    }

    public IEnumerable<string> GetMethodParams(string methodname)
    {
        ArgumentNullException.ThrowIfNull(methodname);
        
        var method = _type.GetMethod(methodname, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
        if (method == null)
        {
            return Enumerable.Empty<string>();
        }

        return method.GetParameters()
                    .Select(p => p.Name!)
                    .Append(method.ReturnType.Name);
    }

    public IEnumerable<string> GetAllFields()
    {
        return _type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                    .Select(f => f.Name);
    }

    public IEnumerable<string> GetProperties()
    {
        return _type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                    .Select(p => p.Name);
    }

    public bool HasAttribute<T>() where T : Attribute
    {
        return _type.IsDefined(typeof(T), false);
    }
}