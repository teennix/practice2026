using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using task05;

namespace task05tests;

public class TestClass
{
    public int PublicField;
    private string _privateField = string.Empty;
    public int Property { get; set; }

    public void Method() { }
    public int ParametrizedMethod(string name, int age) => 0;
}

[Serializable]
public class AttributedClass { }

public class ClassAnalyzerTests
{
    [Fact]
    public void Constructor_NullType_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ClassAnalyzer(null!));
    }

    [Fact]
    public void GetPublicMethods_ReturnsCorrectMethods()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var methods = analyzer.GetPublicMethods();

        Assert.Contains("Method", methods);
        Assert.Contains("ParametrizedMethod", methods);
    }

    [Fact]
    public void GetAllFields_IncludesPrivateFields()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var fields = analyzer.GetAllFields();

        Assert.Contains("_privateField", fields);
    }

    [Fact]
    public void GetProperties_ReturnsCorrectProperties()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var properties = analyzer.GetProperties();

        Assert.Contains("Property", properties);
    }

    [Fact]
    public void HasAttribute_ReturnsTrueWhenAttributeExists()
    {
        var analyzer = new ClassAnalyzer(typeof(AttributedClass));
        Assert.True(analyzer.HasAttribute<SerializableAttribute>());
    }

    [Fact]
    public void HasAttribute_ReturnsFalseWhenAttributeDoesNotExist()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        Assert.False(analyzer.HasAttribute<SerializableAttribute>());
    }

    [Fact]
    public void GetMethodParams_ReturnsParametersAndReturnType()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var parameters = analyzer.GetMethodParams("ParametrizedMethod").ToList();

        Assert.Contains("name", parameters);
        Assert.Contains("age", parameters);
        Assert.Contains("Int32", parameters);
    }

    [Fact]
    public void GetMethodParams_NullArgument_ThrowsArgumentNullException()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        Assert.Throws<ArgumentNullException>(() => analyzer.GetMethodParams(null!));
    }
}