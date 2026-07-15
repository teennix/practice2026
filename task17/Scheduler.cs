using System.Collections.Generic;

namespace task17;

public interface IScheduler
{
    bool HasCommand();
    ICommand Select();
    void Add(ICommand cmd);
}

public class RoundRobinScheduler : IScheduler
{
    private readonly Queue<ICommand> _queue = new();

    public bool HasCommand() => _queue.Count > 0;

    public ICommand Select() => _queue.Dequeue();

    public void Add(ICommand cmd) => _queue.Enqueue(cmd);
}