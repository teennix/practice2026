using System;
using System.Threading;

namespace task14;

public class DefiniteIntegral
{
    public static double Solve(double a, double b, Func<double, double> function, double step, int threadsnumber)
    {
        double totalIntegral = 0.0;
        
        using Barrier barrier = new Barrier(threadsnumber + 1);
        
        double segmentLength = (b - a) / threadsnumber;

        for (int i = 0; i < threadsnumber; i++)
        {
            int threadIndex = i;
            
            Thread thread = new Thread(() =>
            {
                double localA = a + threadIndex * segmentLength;
                double localB = (threadIndex == threadsnumber - 1) ? b : localA + segmentLength;
                
                double localSum = 0.0;
                double currentX = localA;
                
                while (currentX < localB)
                {
                    double nextX = Math.Min(currentX + step, localB);
                    double h = nextX - currentX;
                    localSum += (function(currentX) + function(nextX)) * h / 2.0;
                    currentX = nextX;
                }
                
                AddDouble(ref totalIntegral, localSum);
                
                barrier.SignalAndWait();
            });
            
            thread.Start();
        }

        barrier.SignalAndWait();

        return totalIntegral;
    }

    public static double SolveSingleThread(double a, double b, Func<double, double> function, double step)
    {
        int stepsCount = (int)Math.Ceiling((b - a) / step);
        if (stepsCount <= 0) return 0.0;

        double h = (b - a) / stepsCount;
        
        double sum = (function(a) + function(b)) / 2.0;
        
        for (int j = 1; j < stepsCount; j++)
        {
            sum += function(a + j * h);
        }
        
        return sum * h;
    }

    private static void AddDouble(ref double location, double value)
    {
        double initialValue, computedValue;
        do
        {
            initialValue = location;
            computedValue = initialValue + value;
        } 
        while (initialValue != Interlocked.CompareExchange(ref location, computedValue, initialValue));
    }
}