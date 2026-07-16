using System.Collections.Generic;

namespace task17;

public interface IScheduler
{
    bool HasTasks { get; }
    void Add(ICommand command);
    void Remove(ICommand command);
    ICommand? Next();
}

public class RoundRobinScheduler : IScheduler
{
    private readonly List<ICommand> _tasks = new();
    private int _currentIndex = 0;
    private readonly object _lock = new object();

    public bool HasTasks
    {
        get
        {
            lock (_lock) return _tasks.Count > 0;
        }
    }

    public void Add(ICommand command)
    {
        lock (_lock)
        {
            _tasks.Add(command);
        }
    }

    public void Remove(ICommand command)
    {
        lock (_lock)
        {
            int index = _tasks.IndexOf(command);
            if (index != -1)
            {
                _tasks.RemoveAt(index);
                if (_currentIndex >= _tasks.Count)
                {
                    _currentIndex = 0;
                }
                else if (index < _currentIndex)
                {
                    _currentIndex--;
                }
            }
        }
    }

    public ICommand? Next()
    {
        lock (_lock)
        {
            if (_tasks.Count == 0) return null;

            var cmd = _tasks[_currentIndex];
            _currentIndex = (_currentIndex + 1) % _tasks.Count;
            return cmd;
        }
    }
}