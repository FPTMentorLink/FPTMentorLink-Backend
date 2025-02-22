using Repositories.Entities;
using Repositories.Repositories.Interfaces;

namespace Repositories.UnitOfWork.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Account> Accounts { get; }
    IGenericRepository<Appointment> Appointments { get; }
    IGenericRepository<Checkpoint> Checkpoints { get; }
    IGenericRepository<CheckpointTask> CheckpointTasks { get; }
    IGenericRepository<AppointmentFeedback> AppointmentFeedbacks { get; }
    IGenericRepository<Lecturer> Lecturers { get; }
    IGenericRepository<Mentor> Mentors { get; }
    IGenericRepository<MentorAvailability> MentorAvailabilities { get; }
    IGenericRepository<Project> Projects { get; }
    IGenericRepository<Proposal> Proposals { get; }
    IGenericRepository<Student> Students { get; }
    IGenericRepository<ProjectStudent> StudentGroups { get; }
    IGenericRepository<Transaction> Transactions { get; }
    IGenericRepository<WeeklyReport> WeeklyReports { get; }

    Task<int> SaveChangesAsync(bool trackAudit = true, bool trackSoftDelete = true);
}