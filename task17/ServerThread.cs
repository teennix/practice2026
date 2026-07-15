using System;
using System.Collections.Concurrent;
using System.Threading;

namespace task17;

public class ServerThread
{
    private readonly BlockingCollection<ICommand> _queue;
    private readonly IScheduler _scheduler;
    private Thread? _thread;
    private readonly Action<ICommand, Exception>? _exceptionHandler;
    private volatile bool _isHardStopRequested;

    public int ThreadId => _thread?.ManagedThreadId ?? -1;

    public ServerThread(IScheduler scheduler, Action<ICommand, Exception>? exceptionHandler = null)
    {
        _scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
        _queue = new BlockingCollection<ICommand>();
        _exceptionHandler = exceptionHandler;
    }

    public void Start()
    {
        if (_thread != null) throw new InvalidOperationException("Поток уже запущен.");
        _thread = new Thread(ProcessCommands) { IsBackground = true };
        _thread.Start();
    }

    public void Enqueue(ICommand command)
    {
        if (!_queue.IsAddingCompleted)
        {
            try { _queue.Add(command); }
            catch (InvalidOperationException) { /* Очередь закрыта */ }
        }
    }

    internal void RequestHardStop()
    {
        _isHardStopRequested = true;
        _queue.CompleteAdding();
    }

    internal void RequestSoftStop()
    {
        _queue.CompleteAdding();
    }

    private void ProcessCommands()
    {
        try
        {
            while (!_isHardStopRequested)
            {
                int timeout = _scheduler.HasCommand() ? 0 : Timeout.Infinite;

                bool gotNewCommand = false;
                try
                {
                    gotNewCommand = _queue.TryTake(out ICommand? cmd, timeout);
                    if (gotNewCommand && cmd != null)
                    {
                        ExecuteSafe(cmd);
                    }
                }
                catch (InvalidOperationException) { /* SoftStop вызван */ }

                if (!gotNewCommand && _scheduler.HasCommand())
                {
                    ExecuteSafe(_scheduler.Select());
                }

                if (_queue.IsCompleted && !_scheduler.HasCommand() && _queue.Count == 0)
                {
                    break;
                }
            }
        }
        catch (ObjectDisposedException) { /* Ожидаемо при жесткой остановке */ }
    }

    private void ExecuteSafe(ICommand command)
    {
        try
        {
            command.Execute();
        }
        catch (Exception ex)
        {
            _exceptionHandler?.Invoke(command, ex);
        }
    }

    public void Join() => _thread?.Join();
}