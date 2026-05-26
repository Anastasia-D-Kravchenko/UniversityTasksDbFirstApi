using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityTasksDbFirstApi.Data;
using UniversityTasksDbFirstApi.DTOs;

namespace UniversityTasksDbFirstApi.Controllers;

[ApiController]
[Route("api/students")]
public class StudentsController : ControllerBase
{
    private readonly UniversityTasksDbContext _db;

    public StudentsController(UniversityTasksDbContext db)
    {
        _db = db;
    }

    [HttpGet("{idStudent:int}/dashboard")]
    public async Task<IActionResult> GetStudentDashboard(int idStudent)
    {
        var student = await _db.Students
            .AsNoTracking()
            .AsSplitQuery()
            .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
            .Include(s => s.Submissions)
                .ThenInclude(sub => sub.Assignment)
            .FirstOrDefaultAsync(s => s.StudentId == idStudent);

        if (student is null)
            return NotFound($"Student {idStudent} not found.");

        var dto = new StudentDashboardDto(
            student.StudentId,
            student.IndexNumber,
            student.FullName,
            student.Email,
            student.IsActive,
            student.HasAcademicEmail(),
            student.Enrollments.Select(e => new EnrollmentSummaryDto(
                e.EnrollmentId,
                e.CourseId,
                e.Course.Code,
                e.Course.Name,
                e.EnrolledAt,
                e.Status)),
            student.Submissions.Select(s => new SubmissionSummaryDto(
                s.SubmissionId,
                s.AssignmentId,
                s.Assignment.Title,
                s.RepositoryUrl,
                s.SubmittedAt,
                s.Status,
                s.Score,
                s.Feedback))
        );

        return Ok(dto);
    }
}
