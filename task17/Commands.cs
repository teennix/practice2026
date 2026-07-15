using System;

namespace task17;

public interface ICommand
{
    void Execute();
}

public class ActionCommand : ICommand
{
    private readonly Action _action;
    public ActionCommand(Action action) => _action = action;
    public void Execute() => _action();
}

public class HardStopCommand : ICommand
{
    private readonly ServerThread _server;
    public HardStopCommand(ServerThread server) => _server = server;
    public void Execute()
    {
        if (Environment.CurrentManagedThreadId != _server.ThreadId)
            throw new InvalidOperationException("Команда должна выполняться в потоке сервера.");
        _server.RequestHardStop();
    }
}

public class SoftStopCommand : ICommand
{
    private readonly ServerThread _server;
    public SoftStopCommand(ServerThread server) => _server = server;
    public void Execute()
    {
        if (Environment.CurrentManagedThreadId != _server.ThreadId)
            throw new InvalidOperationException("Команда должна выполняться в потоке сервера.");
        _server.RequestSoftStop();
    }
}

public class LongRunningCommand : ICommand
{
    private readonly IScheduler _scheduler;
    private int _remainingSteps;
    private readonly Action _stepAction;

    public LongRunningCommand(IScheduler scheduler, int steps, Action stepAction)
    {
        _scheduler = scheduler;
        _remainingSteps = steps;
        _stepAction = stepAction;
    }

    public void Execute()
    {
        if (_remainingSteps > 0)
        {
            _stepAction();
            _remainingSteps--;

            if (_remainingSteps > 0)
            {
                _scheduler.Add(this);
            }
        }
    }
}