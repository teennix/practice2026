using System;
using System.IO;
using System.Linq;
using Xunit;
using FileSystemCommands;

namespace task08tests;

public class FileSystemCommandsTests
{
    [Fact]
    public void DirectorySizeCommand_ShouldCalculateSize()
    {
        var testDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(testDir);
        File.WriteAllText(Path.Combine(testDir, "test1.txt"), "Hello"); // 5 байт
        File.WriteAllText(Path.Combine(testDir, "test2.txt"), "World"); // 5 байт

        var command = new DirectorySizeCommand(testDir);
        command.Execute(); 

        Assert.Equal(10, command.Size);

        Directory.Delete(testDir, true);
    }

    [Fact]
    public void FindFilesCommand_ShouldFindMatchingFiles()
    {
        var testDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(testDir);
        File.WriteAllText(Path.Combine(testDir, "file1.txt"), "Text");
        File.WriteAllText(Path.Combine(testDir, "file2.log"), "Log");

        var command = new FindFilesCommand(testDir, "*.txt");
        command.Execute(); 

        Assert.Single(command.FoundFiles);
        Assert.EndsWith("file1.txt", command.FoundFiles.First());

        Directory.Delete(testDir, true);
    }
}