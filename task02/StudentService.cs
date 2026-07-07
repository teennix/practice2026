// task02/StudentService.cs
using System.Collections.Generic;
using System.Linq;

namespace task02;

public class StudentService
{
    private readonly List<Student> _students;

    public StudentService(List<Student> students)
    {
        _students = students ?? new List<Student>();
    }

    public IEnumerable<Student> GetStudentsByFaculty(string faculty)
        => _students.Where(s => s.Faculty == faculty);

    public IEnumerable<Student> GetStudentsWithMinAverageGrade(double minAverageGrade)
        => _students.Where(s => s.Grades.Any() && s.Grades.Average() >= minAverageGrade);

    public IEnumerable<Student> GetStudentsOrderedByName()
        => _students.OrderBy(s => s.Name);

    public ILookup<string, Student> GroupStudentsByFaculty()
        => _students.ToLookup(s => s.Faculty);

    public string? GetFacultyWithHighestAverageGrade()
        => _students
            .GroupBy(s => s.Faculty)
            .OrderByDescending(g => g.Average(s => s.Grades.Any() ? s.Grades.Average() : 0))
            .Select(g => g.Key)
            .FirstOrDefault();
}