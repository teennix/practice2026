using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace task05
{
    public class ClassAnalyzer
    {
        private readonly Type _type;

        public ClassAnalyzer(Type type)
        {
            _type = type;
        }

        public IEnumerable<string> GetPublicMethods() =>
            _type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                 .Select(m => m.Name);

        public IEnumerable<string> GetMethodParams(string methodname) =>
            _type.GetMethod(methodname) is MethodInfo method
                ? new[] { method.ReturnType.Name }.Concat(method.GetParameters().Select(p => p.Name))
                : Enumerable.Empty<string>();

        public IEnumerable<string> GetAllFields() =>
            _type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                 .Select(f => f.Name);

        public IEnumerable<string> GetProperties() =>
            _type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                 .Select(p => p.Name);

        public bool HasAttribute<T>() where T : Attribute =>
            _type.GetCustomAttributes(typeof(T), false).Any();
    }
}