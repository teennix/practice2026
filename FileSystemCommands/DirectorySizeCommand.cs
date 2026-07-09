using System;
using System.IO;
using System.Linq;
using CommandLib;

namespace FileSystemCommands;

public class DirectorySizeCommand : ICommand
{
    private readonly string _path;
    public long Size { get; private set; }

    public DirectorySizeCommand(string path)
    {
        ArgumentNullException.ThrowIfNull(path);
        _path = path;
    }

    public void Execute()
    {
        if (!Directory.Exists(_path)) return;
        var dirInfo = new DirectoryInfo(_path);
        Size = dirInfo.EnumerateFiles("*", SearchOption.AllDirectories).Sum(file => file.Length);
    }
}