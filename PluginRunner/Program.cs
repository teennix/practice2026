using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using PluginContracts;

namespace PluginRunner;

class Program
{
    static void Main()
    {
        string pluginsDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        Console.WriteLine($"Поиск плагинов в директории: {pluginsDirectory}\n");

        if (!Directory.Exists(pluginsDirectory))
        {
            Console.WriteLine("Ошибка: Директория не найдена.");
            return;
        }

        string[] dllFiles = Directory.GetFiles(pluginsDirectory, "*.dll", SearchOption.AllDirectories);
        var loadedPluginTypes = new List<Type>();

        foreach (var file in dllFiles)
        {
            try
            {
                Assembly assembly = Assembly.LoadFrom(file);
                var types = assembly.GetTypes()
                    .Where(t => typeof(ICommand).IsAssignableFrom(t) &&
                                t.IsClass &&
                                !t.IsAbstract &&
                                t.GetCustomAttribute<PluginLoadAttribute>() != null);
                
                loadedPluginTypes.AddRange(types);
            }
            catch (BadImageFormatException)
            {
                // Игнорируем библиотеки, не являющиеся .NET сборками
            }
            catch (ReflectionTypeLoadException ex)
            {
                Console.WriteLine($"[Ошибка загрузки типов] {Path.GetFileName(file)}: Не удалось загрузить один или несколько типов из сборки.");
                foreach (var loaderException in ex.LoaderExceptions)
                {
                    Console.WriteLine($"  -> {loaderException?.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Системная ошибка] Не удалось загрузить сборку {Path.GetFileName(file)}: {ex.Message}");
            }
        }

        if (loadedPluginTypes.Count == 0)
        {
            Console.WriteLine("Плагины не найдены.");
            return;
        }

        List<Type> sortedPlugins;
        try
        {
            sortedPlugins = DependencyGraph.TopologicalSort(loadedPluginTypes);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"[Ошибка графа зависимостей] {ex.Message}");
            return;
        }

        Console.WriteLine("Порядок загрузки плагинов: " + string.Join(" -> ", sortedPlugins.Select(t => t.Name)));
        Console.WriteLine(new string('-', 40));

        foreach (var type in sortedPlugins)
        {
            try
            {
                var pluginInstance = (ICommand)Activator.CreateInstance(type)!;
                pluginInstance.Execute();
            }
            catch (MissingMethodException)
            {
                Console.WriteLine($"[Ошибка выполнения] Плагин {type.Name} не имеет конструктора без параметров.");
            }
            catch (TargetInvocationException ex)
            {
                Console.WriteLine($"[Сбой внутри плагина] Плагин {type.Name} выбросил исключение: {ex.InnerException?.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Критический сбой] Ошибка инициализации плагина {type.Name}: {ex.Message}");
            }
        }
    }
}