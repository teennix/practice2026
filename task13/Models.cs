using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace task13;

public class Subject
{
    [Required(ErrorMessage = "Название предмета обязательно.")]
    public string Name { get; set; } = string.Empty;

    [Range(1, 100, ErrorMessage = "Оценка должна быть в диапазоне от 1 до 100.")]
    public int Grade { get; set; }
}

public class Student
{
    [Required(ErrorMessage = "Имя обязательно.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Фамилия обязательна.")]
    public string LastName { get; set; } = string.Empty;

    [JsonConverter(typeof(CustomDateTimeConverter))]
    public DateTime BirthDate { get; set; }

    public List<Subject>? Grades { get; set; }
}