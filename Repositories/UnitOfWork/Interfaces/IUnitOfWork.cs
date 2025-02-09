using Repositories.Entities;
using Repositories.Repositories.Interfaces;

namespace Repositories.UnitOfWork.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Account> Accounts { get; }
    IGenericRepository<Appointment> Appointments { get; }
    IGenericRepository<Checkpoint> Checkpoints { get; }
    IGenericRepository<CheckpointTask> CheckpointTasks { get; }
    IGenericRepository<Feedback> Feedbacks { get; }
    IGenericRepository<Group> Groups { get; }
    IGenericRepository<Lecturer> Lecturers { get; }
    IGenericRepository<Mentor> Mentors { get; }
    IGenericRepository<MentorAvailability> MentorAvailabilities { get; }
    IGenericRepository<Project> Projects { get; }
    IGenericRepository<Proposal> Proposals { get; }
    IGenericRepository<Student> Students { get; }
    IGenericRepository<StudentGroup> StudentGroups { get; }
    IGenericRepository<TaskLog> TaskLogs { get; }
    IGenericRepository<Transactions> Transactions { get; }
    IGenericRepository<WeeklyReports> WeeklyReports { get; }

    Task<int> SaveChangesAsync(bool trackAudit = true, bool trackSoftDelete = true);
}