using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace task13;

public static class StudentSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true,
        Converters = { new CustomDateTimeConverter() },
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
    };

    public static string Serialize(Student student)
    {
        return JsonSerializer.Serialize(student, Options);
    }

    public static Student Deserialize(string json)
    {
        var student = JsonSerializer.Deserialize<Student>(json, Options) 
            ?? throw new JsonException("Не удалось десериализовать JSON в объект Student.");
        
        Validate(student);
        return student;
    }

    public static void SaveToFile(Student student, string filePath)
    {
        var json = Serialize(student);
        File.WriteAllText(filePath, json);
    }

    public static Student LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Файл {filePath} не найден.");

        var json = File.ReadAllText(filePath);
        return Deserialize(json);
    }

    private static void Validate(Student student)
    {
        var context = new ValidationContext(student);
        var results = new List<ValidationResult>();

        // Валидация объекта Student
        if (!Validator.TryValidateObject(student, context, results, true))
        {
            var errors = string.Join("; ", results.Select(r => r.ErrorMessage));
            throw new ValidationException($"Ошибка валидации Student: {errors}");
        }

        if (student.Grades != null)
        {
            foreach (var subject in student.Grades)
            {
                var subjContext = new ValidationContext(subject);
                if (!Validator.TryValidateObject(subject, subjContext, results, true))
                {
                    var errors = string.Join("; ", results.Select(r => r.ErrorMessage));
                    throw new ValidationException($"Ошибка валидации Subject: {errors}");
                }
            }
        }
    }
}