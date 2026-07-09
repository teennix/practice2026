using System;
using System.Collections.Generic;
using System.Linq;

namespace PluginRunner;

public static class DependencyGraph
{
    public static List<Type> TopologicalSort(IEnumerable<Type> plugins)
    {
        var pluginList = plugins.ToList();
        var inDegree = new Dictionary<string, int>();
        var adjacencyList = new Dictionary<string, List<string>>();
        var typeMap = new Dictionary<string, Type>();

        // Инициализация графа
        foreach (var type in pluginList)
        {
            inDegree[type.Name] = 0;
            adjacencyList[type.Name] = new List<string>();
            typeMap[type.Name] = type;
        }

        // Построение ребер
        foreach (var type in pluginList)
        {
            var dependencies = type.GetCustomAttributes(typeof(PluginContracts.PluginDependencyAttribute), false)
                                   .Cast<PluginContracts.PluginDependencyAttribute>();

            foreach (var dep in dependencies)
            {
                if (!typeMap.ContainsKey(dep.DependencyName))
                {
                    throw new InvalidOperationException($"Критическая ошибка: Плагин {type.Name} требует {dep.DependencyName}, но он не найден.");
                }

                adjacencyList[dep.DependencyName].Add(type.Name);
                inDegree[type.Name]++;
            }
        }

        // Обход по алгоритму Кана
        var queue = new Queue<string>(inDegree.Where(kvp => kvp.Value == 0).Select(kvp => kvp.Key));
        var sorted = new List<Type>();

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            sorted.Add(typeMap[current]);

            foreach (var neighbor in adjacencyList[current])
            {
                inDegree[neighbor]--;
                if (inDegree[neighbor] == 0)
                {
                    queue.Enqueue(neighbor);
                }
            }
        }

        if (sorted.Count != pluginList.Count)
        {
            throw new InvalidOperationException("Обнаружена циклическая зависимость между плагинами.");
        }

        return sorted;
    }
}