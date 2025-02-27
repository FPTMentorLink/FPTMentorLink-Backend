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
    IGenericRepository<ProjectStudent> ProjectStudents { get; }
    IGenericRepository<Proposal> Proposals { get; }
    IGenericRepository<Student> Students { get; }

    IGenericRepository<Transaction> Transactions { get; }
    IGenericRepository<WeeklyReport> WeeklyReports { get; }

    IGenericRepository<Term> Terms { get; }

    Task<int> SaveChangesAsync(bool trackAudit = true, bool trackSoftDelete = true);
}