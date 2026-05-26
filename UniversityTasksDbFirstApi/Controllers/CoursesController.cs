using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityTasksDbFirstApi.Data;
using UniversityTasksDbFirstApi.DTOs;

namespace UniversityTasksDbFirstApi.Controllers;

[ApiController]
[Route("api/courses")]
public class CoursesController : ControllerBase
{
    private readonly UniversityTasksDbContext _db;

    public CoursesController(UniversityTasksDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetCourses([FromQuery] bool activeOnly = false)
    {
        var query = _db.Courses.AsNoTracking();

        if (activeOnly)
            query = query.Where(c => c.IsActive);

        var courses = await query
            .Select(c => new CourseDto(
                c.CourseId,
                c.Code,
                c.Name,
                c.Credits,
                c.IsActive,
                c.Assignments.Count))
            .ToListAsync();

        return Ok(courses);
    }

    [HttpGet("{idCourse:int}/assignments")]
    public async Task<IActionResult> GetAssignmentsForCourse(
        int idCourse,
        [FromQuery] bool publishedOnly = false)
    {
        var courseExists = await _db.Courses
            .AsNoTracking()
            .AnyAsync(c => c.CourseId == idCourse);

        if (!courseExists)
            return NotFound($"Course {idCourse} not found.");

        var query = _db.Assignments
            .AsNoTracking()
            .Where(a => a.CourseId == idCourse);

        if (publishedOnly)
            query = query.Where(a => a.IsPublished);

        var assignments = await query
            .Select(a => new AssignmentDto(
                a.AssignmentId,
                a.Title,
                a.Description,
                a.DueDate,
                a.MaxPoints,
                a.IsPublished,
                a.Submissions.Count))
            .ToListAsync();

        return Ok(assignments);
    }
}
