using System;
using System.Collections.Generic;
using Xunit;
using PluginRunner;
using PluginContracts;

namespace task10tests;

public class DependencyGraphTests
{
    [PluginLoad]
    private class NodeA : ICommand { public void Execute() { } }

    [PluginLoad]
    [PluginDependency("NodeA")]
    private class NodeB : ICommand { public void Execute() { } }

    [PluginLoad]
    [PluginDependency("NodeB")]
    private class NodeC : ICommand { public void Execute() { } }

    [PluginLoad]
    [PluginDependency("NodeE")]
    private class NodeD : ICommand { public void Execute() { } }

    [PluginLoad]
    [PluginDependency("NodeD")]
    private class NodeE : ICommand { public void Execute() { } }

    [Fact]
    public void TopologicalSort_ValidLinearGraph_ReturnsCorrectOrder()
    {
        var unsorted = new List<Type> { typeof(NodeC), typeof(NodeA), typeof(NodeB) };
        
        var sorted = DependencyGraph.TopologicalSort(unsorted);

        Assert.Equal(typeof(NodeA), sorted[0]);
        Assert.Equal(typeof(NodeB), sorted[1]);
        Assert.Equal(typeof(NodeC), sorted[2]);
    }

    [Fact]
    public void TopologicalSort_CircularDependency_ThrowsInvalidOperationException()
    {
        var unsorted = new List<Type> { typeof(NodeD), typeof(NodeE) };

        Assert.Throws<InvalidOperationException>(() => DependencyGraph.TopologicalSort(unsorted));
    }
}