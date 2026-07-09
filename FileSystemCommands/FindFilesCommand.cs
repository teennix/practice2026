using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using CommandLib;

namespace FileSystemCommands;

public class FindFilesCommand : ICommand
{
    private readonly string _path;
    private readonly string _pattern;
    public IEnumerable<string> FoundFiles { get; private set; } = Enumerable.Empty<string>();

    public FindFilesCommand(string path, string pattern)
    {
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(pattern);
        _path = path;
        _pattern = pattern;
    }

    public void Execute()
    {
        if (!Directory.Exists(_path)) return;
        FoundFiles = Directory.EnumerateFiles(_path, _pattern, SearchOption.AllDirectories).ToList();
    }
}