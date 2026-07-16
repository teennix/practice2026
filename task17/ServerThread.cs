using System;
using System.Collections.Concurrent;
using System.Threading;

namespace task17;

public class ServerThread
{
    private readonly BlockingCollection<ICommand> _queue = new();
    private readonly IScheduler _scheduler;
    private readonly Action<ICommand, Exception>? _exceptionHandler;
    
    private Thread? _thread;
    private volatile bool _isRunning;
    private volatile bool _softStop;

    public Thread? Thread => _thread;

    public ServerThread(IScheduler scheduler, Action<ICommand, Exception>? exceptionHandler = null)
    {
        _scheduler = scheduler;
        _exceptionHandler = exceptionHandler;
    }

    public void Start()
    {
        _isRunning = true;
        _softStop = false;
        _thread = new Thread(Loop) { IsBackground = true };
        _thread.Start();
    }

    public void Enqueue(ICommand command) => _queue.Add(command);

    public void StopImmediate() => _isRunning = false;

    public void StopGraceful() => _softStop = true;

    public bool Join(int timeout = Timeout.Infinite) => _thread?.Join(timeout) ?? true;

    private void Loop()
    {
        while (_isRunning)
        {
            ICommand? currentCmd = null;
            try
            {
                if (_softStop && _queue.Count == 0 && !_scheduler.HasTasks)
                {
                    _isRunning = false;
                    break;
                }

                int timeout = _scheduler.HasTasks ? 10 : Timeout.Infinite;

                if (_queue.TryTake(out currentCmd, timeout))
                {
                    if (currentCmd is IRepeatableCommand repeatable)
                    {
                        _scheduler.Add(repeatable);
                    }
                    else
                    {
                        currentCmd.Execute();
                    }
                }

                var scheduledCmd = _scheduler.Next();
                if (scheduledCmd != null)
                {
                    currentCmd = scheduledCmd; 
                    try
                    {
                        scheduledCmd.Execute();
                    }
                    finally
                    {
                        if (scheduledCmd is IRepeatableCommand r && r.IsCompleted)
                        {
                            _scheduler.Remove(scheduledCmd);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (_exceptionHandler != null && currentCmd != null)
                    _exceptionHandler(currentCmd, ex);
            }
        }
    }
}