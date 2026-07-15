using System;

namespace task17;

public interface ICommand
{
    void Execute();
}

public class ActionCommand : ICommand
{
    private readonly Action _action;
    public ActionCommand(Action action) => _action = action ?? throw new ArgumentNullException(nameof(action));
    public void Execute() => _action();
}

public class HardStopCommand : ICommand
{
    private readonly ServerThread _serverThread;
    public HardStopCommand(ServerThread serverThread) => _serverThread = serverThread;

    public void Execute()
    {
        if (Environment.CurrentManagedThreadId != _serverThread.ThreadId)
            throw new InvalidOperationException("HardStopCommand может быть вызвана только из потока, который она останавливает.");
        
        _serverThread.RequestHardStop();
    }
}

public class SoftStopCommand : ICommand
{
    private readonly ServerThread _serverThread;
    public SoftStopCommand(ServerThread serverThread) => _serverThread = serverThread;

    public void Execute()
    {
        if (Environment.CurrentManagedThreadId != _serverThread.ThreadId)
            throw new InvalidOperationException("SoftStopCommand может быть вызвана только из потока, который она останавливает.");

        _serverThread.RequestSoftStop();
    }
}