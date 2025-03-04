using Repositories.Entities;
using Repositories.Repositories.Interfaces;

namespace Repositories.UnitOfWork.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Account> Accounts { get; }
    IGenericRepository<Appointment> Appointments { get; }
    IGenericRepository<AppointmentFeedback> AppointmentFeedbacks { get; }
    IGenericRepository<Checkpoint> Checkpoints { get; }
    IGenericRepository<CheckpointTask> CheckpointTasks { get; }
    IGenericRepository<Faculty> Faculties { get; }
    IGenericRepository<Lecturer> Lecturers { get; }
    IGenericRepository<LecturingProposal> LecturingProposals { get; }
    IGenericRepository<Mentor> Mentors { get; }
    IGenericRepository<MentorAvailability> MentorAvailabilities { get; }
    IGenericRepository<MentorFeedback> MentorFeedbacks { get; }
    IGenericRepository<MentoringProposal> MentoringProposals { get; }
    IGenericRepository<Notification> Notifications { get; }
    IGenericRepository<Project> Projects { get; }
    IGenericRepository<ProjectStudent> ProjectStudents { get; }
    IGenericRepository<Proposal> Proposals { get; }
    IGenericRepository<Student> Students { get; }
    IGenericRepository<Term> Terms { get; }
    IGenericRepository<Transaction> Transactions { get; }
    IGenericRepository<WeeklyReport> WeeklyReports { get; }


    Task<int> SaveChangesAsync(bool trackAudit = true, bool trackSoftDelete = true);
}