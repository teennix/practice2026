using System;
using System.Threading;
using task17;
using Xunit;

namespace task17tests;

public class ServerThreadTests
{
    [Fact]
    public void StopCommands_ExecutedOnWrongThread_ThrowException()
    {
        var server = new ServerThread();
        server.Start();
        var hardStop = new HardStopCommand(server);
        var softStop = new SoftStopCommand(server);

        Assert.Throws<InvalidOperationException>(() => hardStop.Execute());
        Assert.Throws<InvalidOperationException>(() => softStop.Execute());

        server.Enqueue(new HardStopCommand(server));
        server.Join();
    }

    [Fact]
    public void HardStop_StopsImmediately_IgnoresRemainingCommands()
    {
        var server = new ServerThread();
        bool taskExecuted = false;

        server.Enqueue(new HardStopCommand(server));
        server.Enqueue(new ActionCommand(() => taskExecuted = true));

        server.Start();
        server.Join();

        Assert.False(taskExecuted);
    }

    [Fact]
    public void SoftStop_DrainsQueueBeforeStopping()
    {
        var server = new ServerThread();
        int executedCommandsCount = 0;

        server.Enqueue(new ActionCommand(() => Interlocked.Increment(ref executedCommandsCount)));
        server.Enqueue(new SoftStopCommand(server));
        
        server.Start();
        server.Join(); 

        server.Enqueue(new ActionCommand(() => Interlocked.Increment(ref executedCommandsCount)));

        Assert.Equal(1, executedCommandsCount);
    }

    [Fact]
    public void ExceptionHandler_CatchesExceptionsFromCommands()
    {
        Exception? caughtException = null;
        var server = new ServerThread((cmd, ex) => caughtException = ex);
        
        server.Enqueue(new ActionCommand(() => throw new DivideByZeroException("Test Exception")));
        server.Enqueue(new HardStopCommand(server));

        server.Start();
        server.Join();

        Assert.NotNull(caughtException);
        Assert.IsType<DivideByZeroException>(caughtException!);
    }
}