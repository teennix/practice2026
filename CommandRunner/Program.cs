using System;
using System.IO;
using System.Linq;
using System.Reflection;
using CommandLib;

namespace CommandRunner;

class Program
{
    static void Main()
    {
        string dllPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "FileSystemCommands", "bin", "Debug", "net11.0", "FileSystemCommands.dll"));

        if (!File.Exists(dllPath))
        {
            Console.WriteLine($"Не найдена библиотека по пути: {dllPath}");
            Console.WriteLine("Убедитесь, что проект FileSystemCommands был скомпилирован.");
            return;
        }

        Assembly assembly = Assembly.LoadFrom(dllPath);
        var commandTypes = assembly.GetTypes()
            .Where(t => typeof(ICommand).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .ToList();

        string currentDir = Directory.GetCurrentDirectory();

        var sizeCommandType = commandTypes.FirstOrDefault(t => t.Name == "DirectorySizeCommand");
        if (sizeCommandType != null)
        {
            ICommand sizeCmd = (ICommand)Activator.CreateInstance(sizeCommandType, currentDir)!;
            sizeCmd.Execute();
            
            var sizeProp = sizeCommandType.GetProperty("Size");
            long size = (long)sizeProp!.GetValue(sizeCmd)!;
            Console.WriteLine($"Размер текущего каталога: {size} байт");
        }

        var findCommandType = commandTypes.FirstOrDefault(t => t.Name == "FindFilesCommand");
        if (findCommandType != null)
        {
            ICommand findCmd = (ICommand)Activator.CreateInstance(findCommandType, currentDir, "*.cs")!;
            findCmd.Execute();
            
            var filesProp = findCommandType.GetProperty("FoundFiles");
            var files = (System.Collections.Generic.IEnumerable<string>)filesProp!.GetValue(findCmd)!;
            Console.WriteLine($"Найдено файлов .cs: {files.Count()}");
        }
    }
}