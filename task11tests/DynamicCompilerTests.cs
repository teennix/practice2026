using System;
using Xunit;
using task11;

namespace task11tests;

public class DynamicCompilerTests
{
    private readonly string _validCode = @"
public class Calculator
{
    public int Add(int a, int b) => a + b;
    public int Minus(int a, int b) => a - b;
    public int Mul(int a, int b) => a * b;
    public int Div(int a, int b) => a / b;
}";

    private readonly string _invalidCode = @"
public class Calculator
{
    public int Add(int a, int b) => a + b // Пропущена точка с запятой
    public int Minus(int a, int b) => a - b;
    public int Mul(int a, int b) => a * b;
    public int Div(int a, int b) => a / b;
}";

    [Fact]
    public void CreateCalculator_ValidCode_MethodsExecuteCorrectly()
    {
        ICalculator calculator = DynamicCompiler.CreateCalculator(_validCode);

        Assert.Equal(5, calculator.Add(2, 3));
        Assert.Equal(5, calculator.Minus(10, 5));
        Assert.Equal(20, calculator.Mul(4, 5));
        Assert.Equal(4, calculator.Div(12, 3));
    }

    [Fact]
    public void CreateCalculator_InvalidCode_ThrowsCompilationException()
    {
        var exception = Assert.Throws<InvalidOperationException>(() => DynamicCompiler.CreateCalculator(_invalidCode));
        Assert.Contains("Ошибка компиляции", exception.Message);
    }
}