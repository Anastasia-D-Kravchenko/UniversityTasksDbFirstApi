using Microsoft.EntityFrameworkCore;
using UniversityTasksDbFirstApi.Data;
using UniversityTasksDbFirstApi.DTOs;
using UniversityTasksDbFirstApi.Models;

namespace UniversityTasksDbFirstApi.Services;

public class SubmissionService
{
    private readonly UniversityTasksDbContext _db;

    public SubmissionService(UniversityTasksDbContext db)
    {
        _db = db;
    }

    public async Task<int> CreateSubmissionAsync(CreateSubmissionDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.RepositoryUrl) ||
            !dto.RepositoryUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("RepositoryUrl must be non-empty and start with 'https://'.");
        }

        var student = await _db.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.StudentId == dto.StudentId)
            ?? throw new KeyNotFoundException($"Student {dto.StudentId} not found.");

        if (!student.IsActive)
            throw new InvalidOperationException($"Student {dto.StudentId} is not active.");

        var assignment = await _db.Assignments
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.AssignmentId == dto.AssignmentId)
            ?? throw new KeyNotFoundException($"Assignment {dto.AssignmentId} not found.");

        if (!assignment.IsPublished)
            throw new InvalidOperationException($"Assignment {dto.AssignmentId} is not published.");

        var enrolled = await _db.Enrollments
            .AsNoTracking()
            .AnyAsync(e =>
                e.StudentId == dto.StudentId &&
                e.CourseId == assignment.CourseId &&
                (e.Status == "Active" || e.Status == "Completed"));

        if (!enrolled)
            throw new InvalidOperationException(
                $"Student {dto.StudentId} is not enrolled (Active/Completed) in the course for assignment {dto.AssignmentId}.");

        var duplicate = await _db.Submissions
            .AsNoTracking()
            .AnyAsync(s => s.AssignmentId == dto.AssignmentId && s.StudentId == dto.StudentId);

        if (duplicate)
            throw new InvalidOperationException(
                $"Student {dto.StudentId} has already submitted assignment {dto.AssignmentId}.");

        var now = DateTime.UtcNow;
        var status = assignment.IsOverdue(now) ? "Late" : "Submitted";

        var submission = new Submission
        {
            AssignmentId = dto.AssignmentId,
            StudentId = dto.StudentId,
            RepositoryUrl = dto.RepositoryUrl,
            SubmittedAt = now,
            Status = status
        };

        _db.Submissions.Add(submission);
        await _db.SaveChangesAsync();

        return submission.SubmissionId;
    }

    public async Task GradeSubmissionAsync(int submissionId, GradeSubmissionDto dto)
    {
        var submission = await _db.Submissions
            .Include(s => s.Assignment)
            .FirstOrDefaultAsync(s => s.SubmissionId == submissionId)
            ?? throw new KeyNotFoundException($"Submission {submissionId} not found.");

        if (submission.Status == "Graded")
            throw new InvalidOperationException($"Submission {submissionId} is already graded.");

        if (dto.Score < 0)
            throw new ArgumentException("Score cannot be negative.");

        if (dto.Score > submission.Assignment.MaxPoints)
            throw new ArgumentException(
                $"Score {dto.Score} exceeds MaxPoints ({submission.Assignment.MaxPoints}).");

        submission.Score = dto.Score;
        submission.Feedback = dto.Feedback;
        _db.Entry(submission).Property(s => s.Status).IsModified = true;
        submission.Status = "Graded";

        await _db.SaveChangesAsync();
    }

    public async Task DeleteSubmissionAsync(int submissionId)
    {
        var submission = await _db.Submissions
            .FirstOrDefaultAsync(s => s.SubmissionId == submissionId)
            ?? throw new KeyNotFoundException($"Submission {submissionId} not found.");

        if (submission.Status == "Graded")
            throw new InvalidOperationException("A graded submission cannot be deleted.");

        _db.Submissions.Remove(submission);
        await _db.SaveChangesAsync();
    }
}
