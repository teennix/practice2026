using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace task09;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Ошибка: Укажите путь к DLL в качестве аргумента командной строки.");
            return;
        }

        string dllPath = args[0];

        if (!File.Exists(dllPath))
        {
            Console.WriteLine($"Ошибка: Файл не найден по пути {dllPath}");
            return;
        }

        try
        {
            Assembly assembly = Assembly.LoadFrom(dllPath);
            var classes = assembly.GetTypes().Where(t => t.IsClass).ToList();

            Console.WriteLine($"Метаданные сборки: {assembly.GetName().Name}\n");

            foreach (var cls in classes)
            {
                Console.WriteLine($"Класс: {cls.FullName}");

                // Атрибуты класса
                var attributes = cls.GetCustomAttributes().ToList();
                if (attributes.Any())
                {
                    Console.WriteLine("  Атрибуты:");
                    attributes.ForEach(a => Console.WriteLine($"    - {a.GetType().Name}"));
                }

                // Конструкторы и их параметры
                var constructors = cls.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                if (constructors.Any())
                {
                    Console.WriteLine("  Конструкторы:");
                    foreach (var ctor in constructors)
                    {
                        var ctorParams = string.Join(", ", ctor.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
                        Console.WriteLine($"    - {cls.Name}({ctorParams})");
                    }
                }

                // Методы и их параметры
                var methods = cls.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
                if (methods.Any())
                {
                    Console.WriteLine("  Методы:");
                    foreach (var method in methods)
                    {
                        var methodParams = string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
                        Console.WriteLine($"    - {method.ReturnType.Name} {method.Name}({methodParams})");
                    }
                }
                
                Console.WriteLine(new string('-', 40));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при загрузке библиотеки или чтении метаданных: {ex.Message}");
        }
    }
}