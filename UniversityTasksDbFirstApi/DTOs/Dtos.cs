namespace UniversityTasksDbFirstApi.DTOs;

public record CourseDto(
    int CourseId,
    string Code,
    string Name,
    int Credits,
    bool IsActive,
    int AssignmentCount
);

public record AssignmentDto(
    int AssignmentId,
    string Title,
    string? Description,
    DateTime DueDate,
    int MaxPoints,
    bool IsPublished,
    int SubmissionCount
);

public record EnrollmentSummaryDto(
    int EnrollmentId,
    int CourseId,
    string CourseCode,
    string CourseName,
    DateOnly EnrolledAt,
    string Status
);

public record SubmissionSummaryDto(
    int SubmissionId,
    int AssignmentId,
    string AssignmentTitle,
    string RepositoryUrl,
    DateTime SubmittedAt,
    string Status,
    int? Score,
    string? Feedback
);

public record StudentDashboardDto(
    int StudentId,
    string IndexNumber,
    string FullName,
    string Email,
    bool IsActive,
    bool HasAcademicEmail,
    IEnumerable<EnrollmentSummaryDto> Enrollments,
    IEnumerable<SubmissionSummaryDto> Submissions
);

public record SubmissionDto(
    int SubmissionId,
    int StudentId,
    string StudentFullName,
    int AssignmentId,
    string AssignmentTitle,
    string RepositoryUrl,
    DateTime SubmittedAt,
    string Status,
    int? Score,
    string? Feedback
);

public record CreateSubmissionDto(
    int AssignmentId,
    int StudentId,
    string RepositoryUrl
);

public record GradeSubmissionDto(
    int Score,
    string? Feedback
);
