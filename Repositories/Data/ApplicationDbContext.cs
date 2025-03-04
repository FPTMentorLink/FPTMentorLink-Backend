using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Repositories.Entities;
using Repositories.Entities.Base;

namespace Repositories.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<AppointmentFeedback> AppointmentFeedbacks { get; set; }
    public DbSet<Checkpoint> Checkpoints { get; set; }
    public DbSet<CheckpointTask> CheckpointTasks { get; set; }
    public DbSet<Faculty> Faculties { get; set; }
    public DbSet<Lecturer> Lecturers { get; set; }
    public DbSet<LecturingProposal> LecturingProposals { get; set; }
    public DbSet<Mentor> Mentors { get; set; }
    public DbSet<MentorAvailability> MentorAvailabilities { get; set; }
    public DbSet<MentorFeedback> MentorFeedbacks { get; set; }
    public DbSet<MentoringProposal> MentoringProposals { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectStudent> ProjectStudents { get; set; }
    public DbSet<Proposal> Proposals { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Term> Terms { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<WeeklyReport> WeeklyReports { get; set; }
    public DbSet<WeeklyReportFeedback> WeeklyReportFeedbacks { get; set; }


    //Archived tables
    public DbSet<ArchiveAppointment> ArchiveAppointments { get; set; }
    public DbSet<ArchiveCheckpointTask> ArchiveCheckpointTasks { get; set; }
    public DbSet<ArchiveProjectStudent> ArchiveProjectStudents { get; set; }

    private static void SoftDeleteFilter(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType)) continue;
            var parameter = Expression.Parameter(entityType.ClrType, "p");
            var property = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
            var condition = Expression.Equal(property, Expression.Constant(false));
            var lambda = Expression.Lambda(condition, parameter);

            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        SoftDeleteFilter(modelBuilder);
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable(nameof(Account));
            // entity.Property(a => a.Roles)
            //     .HasConversion(
            //         // Convert AccountRole[] to string when saving to database
            //         v => string.Join(',', v.Select(r => (int)r)),
            //         // Convert string back to AccountRole[] when reading from database
            //         v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
            //             .Select(r => (AccountRole)int.Parse(r))
            //             .ToArray(),
            //         // Value comparer for proper change tracking
            //         new ValueComparer<AccountRole[]>(
            //             // Equality comparison function
            //             // Determines if two arrays are equal by comparing their elements in sequence
            //             (c1, c2) => c1!.SequenceEqual(c2!),
            //
            //             // Hash code generation function
            //             // Creates a hash code by combining hash codes of all elements
            //             // Used for dictionary keys and hash-based collections
            //             c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            //
            //             // Snapshot function
            //             // Creates a copy of the array for change tracking
            //             // EF Core uses this to detect changes between original and current values
            //             c => c.ToArray()
            //         ));
            entity.Property(p => p.Role)
                .HasConversion(
                    v => v.ToString(),
                    v => (AccountRole)Enum.Parse(typeof(AccountRole), v));
            entity.Property(p => p.Username)
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");
            entity.Property(p => p.Email)
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");
            entity.Property(p => p.FirstName)
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");
            entity.Property(p => p.LastName)
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");


            // Configure one-to-one relationships
            entity.HasOne(a => a.Student)
                .WithOne(s => s.Account)
                .HasForeignKey<Student>(s => s.Id);

            entity.HasOne(a => a.Mentor)
                .WithOne(m => m.Account)
                .HasForeignKey<Mentor>(m => m.Id);

            entity.HasOne(a => a.Lecturer)
                .WithOne(l => l.Account)
                .HasForeignKey<Lecturer>(l => l.Id);
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.ToTable(nameof(Appointment));
            entity.Property(p => p.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (AppointmentStatus)Enum.Parse(typeof(AppointmentStatus), v));
        });

        modelBuilder.Entity<Checkpoint>(entity =>
        {
            entity.ToTable(nameof(Checkpoint));
            entity.Property(p => p.Name)
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");
        });

        modelBuilder.Entity<CheckpointTask>(entity =>
        {
            entity.ToTable(nameof(CheckpointTask));
            entity.Property(p => p.Name)
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");
            entity.Property(p => p.Description)
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");
            entity.Property(p => p.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (CheckpointTaskStatus)Enum.Parse(typeof(CheckpointTaskStatus), v));
        });

        modelBuilder.Entity<Faculty>(entity =>
        {
            entity.ToTable(nameof(Faculty));
            entity.Property(p => p.Name)
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");
        });

        modelBuilder.Entity<AppointmentFeedback>(entity =>
        {
            entity.ToTable(nameof(AppointmentFeedback));
            entity.Property(p => p.Content)
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");
        });

        modelBuilder.Entity<Lecturer>(entity =>
        {
            entity.ToTable(nameof(Lecturer));
            entity.Property(p => p.Description)
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");
        });

        modelBuilder.Entity<LecturingProposal>(entity =>
        {
            entity.ToTable(nameof(LecturingProposal));
            entity.Property(p => p.StudentNote)
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");
            entity.Property(p => p.LecturerNote)
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");
            entity.Property(p => p.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (LecturingProposalStatus)Enum.Parse(typeof(LecturingProposalStatus), v));
        });

        modelBuilder.Entity<Mentor>(entity =>
        {
            entity.ToTable(nameof(Mentor));
            entity.Property(p => p.BankName)
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");
            entity.Property(p => p.BankCode)
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");
        });

        modelBuilder.Entity<MentorAvailability>(entity =>
        {
            entity.ToTable(nameof(MentorAvailability));
            entity.Property(p => p.TimeMap)
                .HasColumnType("binary(12)");
        });

        modelBuilder.Entity<MentorFeedback>(entity =>
        {
            entity.ToTable(nameof(MentorFeedback));
            entity.Property(p => p.Content)
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");
        });

        modelBuilder.Entity<MentoringProposal>(entity =>
        {
            entity.ToTable(nameof(MentoringProposal));
            entity.Property(p => p.StudentNote)
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");
            entity.Property(p => p.MentorNote)
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");
            entity.Property(p => p.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (MentoringProposalStatus)Enum.Parse(typeof(MentoringProposalStatus), v));
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable(nameof(Notification));
            entity.Property(p => p.Content)
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.ToTable(nameof(Project));
            entity.Property(p => p.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (ProjectStatus)Enum.Parse(typeof(ProjectStatus), v));
            entity.Property(p => p.Name)
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");
            entity.Property(p => p.Description)
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");
        });

        modelBuilder.Entity<ProjectStudent>(entity => { entity.ToTable(nameof(ProjectStudent)); });

        modelBuilder.Entity<Proposal>(entity =>
        {
            entity.ToTable(nameof(Proposal));
            entity.Property(p => p.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (ProposalStatus)Enum.Parse(typeof(ProposalStatus), v));
            entity.Property(p => p.Name)
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable(nameof(Student));
            entity.HasOne(a => a.ProjectStudent)
                .WithOne(ps => ps.Student)
                .HasForeignKey<ProjectStudent>(ps => ps.StudentId);
        });

        modelBuilder.Entity<Term>(entity =>
        {
            entity.ToTable(nameof(Term));
            entity.Property(p => p.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (TermStatus)Enum.Parse(typeof(TermStatus), v));
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.ToTable(nameof(Transaction));
            entity.Property(p => p.Type)
                .HasConversion(
                    v => v.ToString(),
                    v => (TransactionType)Enum.Parse(typeof(TransactionType), v));
            entity.Property(p => p.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (TransactionStatus)Enum.Parse(typeof(TransactionStatus), v));
            entity.Property(p => p.TransactionMethod)
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");
        });

        modelBuilder.Entity<WeeklyReport>(entity =>
        {
            entity.ToTable(nameof(WeeklyReport));
            entity.Property(p => p.Title)
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");
            entity.Property(p => p.Content)
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");
        });

        modelBuilder.Entity<WeeklyReportFeedback>(entity =>
        {
            entity.ToTable(nameof(WeeklyReportFeedback));
            entity.Property(p => p.Content)
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");
        });

        modelBuilder.Entity<ArchiveAppointment>(entity =>
        {
            entity.ToTable(nameof(ArchiveAppointment));
            entity.Property(p => p.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (AppointmentStatus)Enum.Parse(typeof(AppointmentStatus), v));
        });

        modelBuilder.Entity<ArchiveCheckpointTask>(entity =>
        {
            entity.ToTable(nameof(ArchiveCheckpointTask));
            entity.Property(p => p.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (CheckpointTaskStatus)Enum.Parse(typeof(CheckpointTaskStatus), v));
        });

        modelBuilder.Entity<ArchiveProjectStudent>(entity => { entity.ToTable(nameof(ArchiveProjectStudent)); });

        base.OnModelCreating(modelBuilder);
    }
}