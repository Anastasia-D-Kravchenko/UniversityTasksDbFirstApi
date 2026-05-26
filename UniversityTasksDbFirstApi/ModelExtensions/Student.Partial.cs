namespace UniversityTasksDbFirstApi.Models;

public partial class Student
{
    public string FullName => $"{FirstName} {LastName}";

    public bool HasAcademicEmail() =>
        Email.EndsWith("@students.example.edu", StringComparison.OrdinalIgnoreCase);
}
