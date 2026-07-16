using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public void Task19_LongRunningOperations_ReportAndGraph()
    {
        var scheduler = new RoundRobinScheduler();
        var server = new ServerThread(scheduler);
        server.Start();

        var executionHistory = new List<(int Step, int CmdId, int CallNum)>();
        var lockObj = new object();
        int stepCounter = 0;
        using var allDoneEvent = new CountdownEvent(15); 
        
        try
        {
            for (int i = 1; i <= 5; i++)
            {
                var cmd = new TestCommand(i, 3, (id, call) =>
                {
                    lock (lockObj)
                    {
                        stepCounter++;
                        executionHistory.Add((stepCounter, id, call));
                    }
                    allDoneEvent.Signal();
                });
                server.Enqueue(cmd);
            }

            bool completed = allDoneEvent.Wait(TimeSpan.FromSeconds(5));
            Assert.True(completed, "Задачи зависли и не успели выполниться за 5 секунд!");
        }
        finally
        {
            server.Enqueue(new HardStopCommand(server));
            
            server.Join(1000); 
        }

        string baseDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../"));
        string reportPath = Path.Combine(baseDir, "task19_report.txt");
        string graphPath = Path.Combine(baseDir, "task19_graph.png");

        using var writer = new StreamWriter(reportPath);
        writer.WriteLine("=== ОТЧЕТ ПО ДЛИТЕЛЬНЫМ ОПЕРАЦИЯМ (ЗАДАЧА 19) ===\n");
        writer.WriteLine("1. Запуск 5 экземпляров TestCommand (по 3 вызова каждый):");

        foreach (var item in executionHistory)
        {
            writer.WriteLine($"[Шаг {item.Step:D2}] Поток (команда) {item.CmdId} -> вызов {item.CallNum}");
        }

        writer.WriteLine("\n2. Отправка команды HardStop...");
        writer.WriteLine("=> Поток сервера успешно остановлен.");
        writer.WriteLine("\n3. Анализ псевдопараллелизма:");
        writer.WriteLine("Команды выполнялись строго по очереди через RoundRobinScheduler.");

        // Построение графика
        Plot plt = new();
        double[] xs = executionHistory.Select(x => (double)x.Step).ToArray();
        double[] ys = executionHistory.Select(x => (double)x.CmdId).ToArray();

        var markers = plt.Add.Markers(xs, ys);
        markers.MarkerSize = 12;
        markers.MarkerShape = MarkerShape.FilledCircle;
        
        var line = plt.Add.ScatterLine(xs, ys);
        line.LineWidth = 1;
        line.Color = Colors.Gray.WithAlpha(0.5);

        plt.Title("Псевдопараллельное выполнение операций");
        plt.XLabel("Порядковый номер шага");
        plt.YLabel("ID команды");

        ScottPlot.TickGenerators.NumericManual yTicks = new();
        for (int i = 1; i <= 5; i++) yTicks.AddMajor(i, $"Команда {i}");
        plt.Axes.Left.TickGenerator = yTicks;

        plt.SavePng(graphPath, 800, 500);
    }
}