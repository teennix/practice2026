using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using task13;
using Xunit;

namespace task13tests;

public class StudentSerializerTests
{
    private readonly Student _validStudent = new()
    {
        FirstName = "Иван",
        LastName = "Иванов",
        BirthDate = new DateTime(2005, 5, 15),
        Grades = new List<Subject>
        {
            new Subject { Name = "Math", Grade = 95 },
            new Subject { Name = "Physics", Grade = 88 }
        }
    };

    [Fact]
    public void Serialize_ShouldFormatDateAndIgnoreNulls()
    {
        var student = new Student
        {
            FirstName = "Анна",
            LastName = "Смирнова",
            BirthDate = new DateTime(2006, 12, 1),
            Grades = null
        };

        var json = StudentSerializer.Serialize(student);

        Assert.Contains("\"2006-12-01\"", json);
        Assert.DoesNotContain("grades", json.ToLower());
    }

    [Fact]
    public void Deserialize_ValidJson_ReturnsObject()
    {
        var json = StudentSerializer.Serialize(_validStudent);

        var result = StudentSerializer.Deserialize(json);

        Assert.Equal("Иван", result.FirstName);
        Assert.Equal(new DateTime(2005, 5, 15), result.BirthDate);
        Assert.Equal(2, result.Grades!.Count);
    }

    [Fact]
    public void Deserialize_InvalidData_ThrowsValidationException()
    {
        var invalidJson = @"
        {
          ""firstName"": ""Петр"",
          ""lastName"": ""Петров"",
          ""birthDate"": ""2005-01-01"",
          ""grades"": [
            {
              ""name"": ""Math"",
              ""grade"": 150 
            }
          ]
        }";

        // Act & Assert
        var exception = Assert.Throws<ValidationException>(() => StudentSerializer.Deserialize(invalidJson));
        Assert.Contains("в диапазоне от 1 до 100", exception.Message);
    }

    [Fact]
    public void SaveAndLoad_FileOperations_WorkCorrectly()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();

        try
        {
            StudentSerializer.SaveToFile(_validStudent, tempFile);
            var loadedStudent = StudentSerializer.LoadFromFile(tempFile);

            Assert.Equal(_validStudent.FirstName, loadedStudent.FirstName);
            Assert.Equal(_validStudent.Grades![0].Name, loadedStudent.Grades![0].Name);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }
}