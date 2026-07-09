using System;
using PluginContracts;

namespace PluginA;

[PluginLoad]
public class FirstPlugin : ICommand
{
    public void Execute()
    {
        Console.WriteLine("FirstPlugin успешно выполнен.");
    }
}