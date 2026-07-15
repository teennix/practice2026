using System;
using System.Collections.Concurrent;
using System.Threading;

namespace task17;

public class ServerThread
{
    private readonly BlockingCollection<ICommand> _queue;
    
    private Thread? _thread; 
    
    private readonly Action<ICommand, Exception>? _exceptionHandler; 
    
    private volatile bool _isHardStopRequested;

    public int ThreadId => _thread?.ManagedThreadId ?? -1;

    public ServerThread(Action<ICommand, Exception>? exceptionHandler = null)
    {
        _queue = new BlockingCollection<ICommand>();
        _exceptionHandler = exceptionHandler; 
    }

    public void Start()
    {
        if (_thread != null) 
            throw new InvalidOperationException("Поток уже запущен.");

        _thread = new Thread(ProcessCommands) { IsBackground = true };
        _thread.Start();
    }

    public void Enqueue(ICommand command)
    {
        if (!_queue.IsAddingCompleted)
        {
            try { _queue.Add(command); }
            catch (InvalidOperationException) { /* Очередь закрылась во время добавления */ }
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
            foreach (var command in _queue.GetConsumingEnumerable())
            {
                if (_isHardStopRequested) break;

                try
                {
                    command.Execute();
                }
                catch (Exception ex)
                {
                    _exceptionHandler?.Invoke(command, ex);
                }
            }
        }
        catch (ObjectDisposedException) { /* Ожидаемо при жесткой очистке */ }
    }

    public void Join() => _thread?.Join();
}