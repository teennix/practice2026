using System;
using System.Linq;
using Xunit;
using task05;

namespace task05tests
{
    public class TestClass
    {
        public int PublicField;
        private string _privateField;
        public int Property { get; set; }

        public void Method() { }
        
        public string MethodWithParams(int x, bool flag) => string.Empty;
    }

    [Serializable]
    public class AttributedClass { }

    public class ClassAnalyzerTests
    {
        [Fact]
        public void GetPublicMethods_ReturnsCorrectMethods()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var methods = analyzer.GetPublicMethods();

            Assert.Contains("Method", methods);
        }

        [Fact]
        public void GetAllFields_IncludesPrivateFields()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var fields = analyzer.GetAllFields();

            Assert.Contains("_privateField", fields);
            Assert.Contains("PublicField", fields);
        }

        [Fact]
        public void GetProperties_ReturnsCorrectProperties()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var properties = analyzer.GetProperties();

            Assert.Contains("Property", properties);
        }

        [Fact]
        public void HasAttribute_ReturnsTrueIfAttributeExists()
        {
            var analyzer = new ClassAnalyzer(typeof(AttributedClass));
            
            Assert.True(analyzer.HasAttribute<SerializableAttribute>());
        }

        [Fact]
        public void HasAttribute_ReturnsFalseIfAttributeDoesNotExist()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            
            Assert.False(analyzer.HasAttribute<SerializableAttribute>());
        }

        [Fact]
        public void GetMethodParams_ReturnsReturnTypeAndParameters()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var parameters = analyzer.GetMethodParams(nameof(TestClass.MethodWithParams)).ToList();

            Assert.Equal(3, parameters.Count);
            Assert.Equal("String", parameters[0]);
            Assert.Equal("x", parameters[1]);
            Assert.Equal("flag", parameters[2]);
        }
    }
}