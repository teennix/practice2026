using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace task11;

public static class DynamicCompiler
{
    public static ICalculator CreateCalculator(string classDefinition)
    {
        string modifiedCode = $@"
using System;
using task11;

namespace DynamicCode
{{
    {classDefinition.Replace("public class Calculator", "public class Calculator : ICalculator")}
}}";

        var syntaxTree = CSharpSyntaxTree.ParseText(modifiedCode);

        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .Cast<MetadataReference>()
            .ToList();

        var compilation = CSharpCompilation.Create("DynamicCalculatorAssembly")
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .AddReferences(references)
            .AddSyntaxTrees(syntaxTree);

        using var ms = new MemoryStream();
        var emitResult = compilation.Emit(ms);

        if (!emitResult.Success)
        {
            var errors = emitResult.Diagnostics
                .Where(d => d.IsWarningAsError || d.Severity == DiagnosticSeverity.Error)
                .Select(d => $"{d.Id}: {d.GetMessage()}");

            string errorMessage = string.Join(Environment.NewLine, errors);
            throw new InvalidOperationException($"Ошибка компиляции сгенерированного кода:\n{errorMessage}");
        }

        ms.Seek(0, SeekOrigin.Begin);
        var assembly = Assembly.Load(ms.ToArray());
        var type = assembly.GetTypes().First(t => typeof(ICalculator).IsAssignableFrom(t));

        return (ICalculator)Activator.CreateInstance(type)!;
    }
}