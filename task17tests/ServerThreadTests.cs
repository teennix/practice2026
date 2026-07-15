using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using task17;
using Xunit;
using Xunit.Abstractions;
using ScottPlot;

namespace task17tests;

public class ServerThreadTests
{
    private readonly ITestOutputHelper _output;

    public ServerThreadTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void StopCommands_ExecutedOnWrongThread_ThrowException()
    {
        var server = new ServerThread(new RoundRobinScheduler());
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
        var server = new ServerThread(new RoundRobinScheduler());
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
        var server = new ServerThread(new RoundRobinScheduler());
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
        var server = new ServerThread(new RoundRobinScheduler(), (cmd, ex) => caughtException = ex);
        
        server.Enqueue(new ActionCommand(() => throw new DivideByZeroException("Test Exception")));
        server.Enqueue(new HardStopCommand(server));

        server.Start();
        server.Join();

        Assert.NotNull(caughtException);
        Assert.IsType<DivideByZeroException>(caughtException!);
    }

    [Fact]
    public void Task18_GenerateReportAndGraph()
    {
        string baseDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../"));
        string reportPath = Path.Combine(baseDir, "task18_report.txt");
        string graphPath = Path.Combine(baseDir, "task18_graph.png");

        using var writer = new StreamWriter(reportPath);
        writer.WriteLine("=== ОТЧЕТ ПО ПЛАНИРОВЩИКУ КОМАНД (ЗАДАЧА 18) ===\n");

        var scheduler = new RoundRobinScheduler();
        var server = new ServerThread(scheduler);
        server.Start();

        var watch = new Stopwatch();
        bool isShortTaskDone = false;

        writer.WriteLine("1. Запуск длительной операции (10 шагов по 50 мс)...");
        var longCommand = new LongRunningCommand(scheduler, 10, () => Thread.Sleep(50));
        server.Enqueue(longCommand); 

        Thread.Sleep(20); 
        
        writer.WriteLine("2. Замер времени отклика на новую (короткую) команду...");
        watch.Start();
        server.Enqueue(new ActionCommand(() => 
        {
            watch.Stop();
            isShortTaskDone = true;
        }));

        server.Enqueue(new SoftStopCommand(server));
        server.Join();

        Assert.True(isShortTaskDone, "Короткая задача не выполнилась из-за блокировки очереди!");

        double asyncResponseTime = watch.ElapsedMilliseconds;
        double syncResponseTime = 500.0;

        writer.WriteLine("\n3. Сравнение времени отклика:");
        writer.WriteLine($"Время ожидания при синхронном выполнении (без планировщика): ~{syncResponseTime} мс");
        writer.WriteLine($"Время ожидания с планировщиком (Round Robin): {asyncResponseTime:F2} мс");
        
        double speedup = syncResponseTime / asyncResponseTime;
        writer.WriteLine($"\n=> Отзывчивость сервера улучшилась примерно в {speedup:F1} раз.");

        Plot plt = new();
        
        double[] positions = { 0, 1 };
        double[] values = { syncResponseTime, asyncResponseTime };
        
        var bars = plt.Add.Bars(positions, values);
        
        ScottPlot.TickGenerators.NumericManual tickGen = new();
        tickGen.AddMajor(0, "Синхронно (Без планировщика)");
        tickGen.AddMajor(1, "Псевдопараллельно (Round Robin)");
        plt.Axes.Bottom.TickGenerator = tickGen;

        plt.Title("Сравнение времени отклика сервера");
        plt.YLabel("Время ожидания (мс)");
        
        plt.SavePng(graphPath, 800, 600);
        
        _output.WriteLine("Анализ завершен. Файлы task18_report.txt и task18_graph.png сохранены в корень проекта.");
    }
}