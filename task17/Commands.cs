using System;
using System.Threading;

namespace task17;

public interface ICommand
{
    void Execute();
}

public interface IRepeatableCommand : ICommand
{
    bool IsCompleted { get; }
}

public class ActionCommand(Action action) : ICommand
{
    public void Execute() => action();
}

public class HardStopCommand(ServerThread server) : ICommand
{
    public void Execute()
    {
        if (Thread.CurrentThread != server.Thread)
            throw new InvalidOperationException("HardStop должен выполняться в потоке сервера!");
        server.StopImmediate();
    }
}

public class SoftStopCommand(ServerThread server) : ICommand
{
    public void Execute()
    {
        if (Thread.CurrentThread != server.Thread)
            throw new InvalidOperationException("SoftStop должен выполняться в потоке сервера!");
        server.StopGraceful();
    }
}

public class TestCommand(int id, int maxExecutions = 3, Action<int, int>? onStep = null) : IRepeatableCommand
{
    private int _counter = 0;
    public bool IsCompleted => _counter >= maxExecutions;
    public int Id => id;

    public void Execute()
    {
        _counter++;
        Console.WriteLine($"Поток {id} вызов {_counter}");
        onStep?.Invoke(id, _counter);
    }
}