using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Lecturer> Lecturers { get; set; }
    public DbSet<Mentor> Mentors { get; set; }
    public DbSet<MentorAvailability> MentorAvailabilities { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Proposal> Proposals { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<StudentGroup> StudentGroups { get; set; }
    public DbSet<CheckpointTask> CheckpointTasks { get; set; }
    public DbSet<TaskLog> TaskLogs { get; set; }
    public DbSet<Transactions> Transactions { get; set; }
    public DbSet<WeeklyReports> WeeklyReports { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>()
            .Property(a => a.Roles)
            .HasConversion(
                // Convert AccountRole[] to string when saving to database
                v => string.Join(',', v.Select(r => (int)r)),
                // Convert string back to AccountRole[] when reading from database
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                     .Select(r => (AccountRole)int.Parse(r))
                     .ToArray(),
                // Value comparer for proper change tracking
                new ValueComparer<AccountRole[]>(
                    // Equality comparison function
                    // Determines if two arrays are equal by comparing their elements in sequence
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    
                    // Hash code generation function
                    // Creates a hash code by combining hash codes of all elements
                    // Used for dictionary keys and hash-based collections
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    
                    // Snapshot function
                    // Creates a copy of the array for change tracking
                    // EF Core uses this to detect changes between original and current values
                    c => c.ToArray()
                )
            );
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.Property(a => a.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (AppointmentStatus)Enum.Parse(typeof(AppointmentStatus), v));
        });
        modelBuilder.Entity<Project>(entity =>
        {
            entity.Property(p => p.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (ProjectStatus)Enum.Parse(typeof(ProjectStatus), v));
        });
        modelBuilder.Entity<CheckpointTask>(entity =>
        {
            entity.Property(p => p.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (CheckpointTaskStatus)Enum.Parse(typeof(CheckpointTaskStatus), v));
        });
        modelBuilder.Entity<Transactions>(entity =>
        {
            entity.Property(p => p.Type)
                .HasConversion(
                    v => v.ToString(),
                    v => (TransactionType)Enum.Parse(typeof(TransactionType), v));
            entity.Property(p => p.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (TransactionStatus)Enum.Parse(typeof(TransactionStatus), v));
        });
        modelBuilder.Entity<Proposal>(entity =>
        {
            entity.Property(p => p.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (ProposalStatus)Enum.Parse(typeof(ProposalStatus), v));
        });

        base.OnModelCreating(modelBuilder);
    }
}