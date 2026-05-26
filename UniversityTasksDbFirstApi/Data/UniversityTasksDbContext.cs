using Microsoft.EntityFrameworkCore;
using UniversityTasksDbFirstApi.Models;

namespace UniversityTasksDbFirstApi.Data;

public partial class UniversityTasksDbContext : DbContext
{
    public UniversityTasksDbContext(DbContextOptions<UniversityTasksDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Student> Students { get; set; }
    public virtual DbSet<Course> Courses { get; set; }
    public virtual DbSet<Enrollment> Enrollments { get; set; }
    public virtual DbSet<Assignment> Assignments { get; set; }
    public virtual DbSet<Submission> Submissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId);
            entity.Property(e => e.StudentId).UseIdentityColumn();
            entity.Property(e => e.IndexNumber).HasMaxLength(20).IsRequired();
            entity.Property(e => e.FirstName).HasMaxLength(80).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(80).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(160).IsRequired();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.HasIndex(e => e.IndexNumber).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId);
            entity.Property(e => e.CourseId).UseIdentityColumn();
            entity.Property(e => e.Code).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(160).IsRequired();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.ToTable(tb => tb.HasCheckConstraint("CK_Courses_Credits", "[Credits] BETWEEN 1 AND 10"));
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => e.EnrollmentId);
            entity.Property(e => e.EnrollmentId).UseIdentityColumn();
            entity.Property(e => e.Status).HasMaxLength(30).IsRequired();
            entity.HasIndex(e => new { e.StudentId, e.CourseId }).IsUnique();
            entity.ToTable(tb => tb.HasCheckConstraint(
                "CK_Enrollments_Status", "[Status] IN (N'Active', N'Completed', N'Dropped')"));

            entity.HasOne(e => e.Student)
                  .WithMany(s => s.Enrollments)
                  .HasForeignKey(e => e.StudentId);

            entity.HasOne(e => e.Course)
                  .WithMany(c => c.Enrollments)
                  .HasForeignKey(e => e.CourseId);
        });

        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.HasKey(e => e.AssignmentId);
            entity.Property(e => e.AssignmentId).UseIdentityColumn();
            entity.Property(e => e.Title).HasMaxLength(160).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.IsPublished).HasDefaultValue(false);
            entity.ToTable(tb => tb.HasCheckConstraint("CK_Assignments_MaxPoints", "[MaxPoints] > 0"));

            entity.HasOne(a => a.Course)
                  .WithMany(c => c.Assignments)
                  .HasForeignKey(a => a.CourseId);
        });

        modelBuilder.Entity<Submission>(entity =>
        {
            entity.HasKey(e => e.SubmissionId);
            entity.Property(e => e.SubmissionId).UseIdentityColumn();
            entity.Property(e => e.RepositoryUrl).HasMaxLength(300).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(30).IsRequired();
            entity.Property(e => e.Feedback).HasMaxLength(1000);
            entity.HasIndex(e => new { e.AssignmentId, e.StudentId }).IsUnique();
            entity.ToTable(tb =>
            {
                tb.HasCheckConstraint("CK_Submissions_Status",
                    "[Status] IN (N'Submitted', N'Late', N'Graded')");
                tb.HasCheckConstraint("CK_Submissions_Score",
                    "[Score] IS NULL OR [Score] >= 0");
            });

            entity.HasOne(s => s.Assignment)
                  .WithMany(a => a.Submissions)
                  .HasForeignKey(s => s.AssignmentId);

            entity.HasOne(s => s.Student)
                  .WithMany(st => st.Submissions)
                  .HasForeignKey(s => s.StudentId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
