using Microsoft.EntityFrameworkCore;
using Repositories.Entities;

namespace Repositories.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Checkpoint> Checkpoints { get; set; }
    public DbSet<CheckpointTask> CheckpointTasks { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<Lecturer> Lecturers { get; set; }
    public DbSet<Mentor> Mentors { get; set; }
    public DbSet<MentorAvailability> MentorAvailabilities { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectStudent> ProjectStudents { get; set; }
    public DbSet<Proposal> Proposals { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Term> Terms { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<WeeklyReport> WeeklyReports { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.ToTable(nameof(Feedback));
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
            entity.Property(p => p.Faculty)
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");
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

        modelBuilder.Entity<MentorAvailability>(entity => { entity.ToTable(nameof(MentorAvailability)); });

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
            entity.Property(p => p.Code)
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");
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

        modelBuilder.Entity<Term>(entity =>
        {
            entity.ToTable(nameof(Term));
            entity.Property(p => p.Code)
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");
        });

        base.OnModelCreating(modelBuilder);
    }
}