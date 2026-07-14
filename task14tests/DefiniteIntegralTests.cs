using System;
using task14;
using Xunit;

namespace task14tests;

public class DefiniteIntegralTests
{
    private static readonly Func<double, double> X = (double x) => x;
    private static readonly Func<double, double> SIN = (double x) => Math.Sin(x);

    [Fact]
    public void Solve_LinearFunction_ReturnsCorrectResult()
    {
        Assert.Equal(0, DefiniteIntegral.Solve(-1, 1, X, 1e-4, 2), 1e-4);
    }

    [Fact]
    public void Solve_SinFunction_ReturnsCorrectResult()
    {
        Assert.Equal(0, DefiniteIntegral.Solve(-1, 1, SIN, 1e-5, 8), 1e-4);
    }

    [Fact]
    public void Solve_LinearFunctionPositiveRange_ReturnsCorrectResult()
    {
        Assert.Equal(12.5, DefiniteIntegral.Solve(0, 5, X, 1e-6, 8), 1e-5);
    }
}