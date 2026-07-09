using System;
using PluginContracts;

namespace PluginB;

[PluginLoad]
[PluginDependency("FirstPlugin")]
public class SecondPlugin : ICommand
{
    public void Execute()
    {
        Console.WriteLine("SecondPlugin успешно выполнен (зависимости удовлетворены).");
    }
}