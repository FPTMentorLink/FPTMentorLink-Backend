using Microsoft.EntityFrameworkCore;
using Repositories.Data;
using Repositories.Entities;
using Repositories.Entities.Base;
using Repositories.Repositories;
using Repositories.Repositories.Interfaces;
using Repositories.UnitOfWork.Interfaces;

namespace Repositories.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IGenericRepository<Account>? _accounts;
    private IGenericRepository<Appointment>? _appointments;
    private IGenericRepository<AppointmentFeedback>? _appointmentFeedbacks;
    private IGenericRepository<Checkpoint>? _checkpoints;
    private IGenericRepository<CheckpointTask>? _checkpointTasks;
    private IGenericRepository<Faculty>? _faculties;
    private IGenericRepository<Lecturer>? _lecturers;
    private IGenericRepository<LecturingProposal>? _lecturingProposals;
    private IGenericRepository<Mentor>? _mentors;
    private IGenericRepository<MentorAvailability>? _mentorAvailabilities;
    private IGenericRepository<MentorFeedback>? _mentorFeedbacks;
    private IGenericRepository<MentoringProposal>? _mentoringProposals;
    private IGenericRepository<Notification>? _notifications;
    private IGenericRepository<Project>? _projects;
    private IGenericRepository<ProjectStudent>? _projectStudents;
    private IGenericRepository<Proposal>? _proposals;
    private IGenericRepository<Student>? _students;
    private IGenericRepository<Term>? _terms;
    private IGenericRepository<Transaction>? _transactions;
    private IGenericRepository<WeeklyReport>? _weeklyReports;
    private IGenericRepository<WeeklyReportFeedback>? _weeklyReportFeedbacks;


    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IGenericRepository<Account> Accounts =>
        _accounts ??= new GenericRepository<Account>(_context);

    public IGenericRepository<Appointment> Appointments =>
        _appointments ??= new GenericRepository<Appointment>(_context);

    public IGenericRepository<Checkpoint> Checkpoints =>
        _checkpoints ??= new GenericRepository<Checkpoint>(_context);

    public IGenericRepository<CheckpointTask> CheckpointTasks =>
        _checkpointTasks ??= new GenericRepository<CheckpointTask>(_context);

    public IGenericRepository<Faculty> Faculties =>
        _faculties ??= new GenericRepository<Faculty>(_context);

    public IGenericRepository<AppointmentFeedback> AppointmentFeedbacks =>
        _appointmentFeedbacks ??= new GenericRepository<AppointmentFeedback>(_context);

    public IGenericRepository<Lecturer> Lecturers =>
        _lecturers ??= new GenericRepository<Lecturer>(_context);

    public IGenericRepository<LecturingProposal> LecturingProposals =>
        _lecturingProposals ??= new GenericRepository<LecturingProposal>(_context);

    public IGenericRepository<Mentor> Mentors =>
        _mentors ??= new GenericRepository<Mentor>(_context);


    public IGenericRepository<MentorAvailability> MentorAvailabilities =>
        _mentorAvailabilities ??= new GenericRepository<MentorAvailability>(_context);

    public IGenericRepository<MentorFeedback> MentorFeedbacks =>
        _mentorFeedbacks ??= new GenericRepository<MentorFeedback>(_context);

    public IGenericRepository<MentoringProposal> MentoringProposals =>
        _mentoringProposals ??= new GenericRepository<MentoringProposal>(_context);

    public IGenericRepository<Notification> Notifications =>
        _notifications ??= new GenericRepository<Notification>(_context);


    public IGenericRepository<Project> Projects =>
        _projects ??= new GenericRepository<Project>(_context);


    public IGenericRepository<Proposal> Proposals =>
        _proposals ??= new GenericRepository<Proposal>(_context);


    public IGenericRepository<Student> Students =>
        _students ??= new GenericRepository<Student>(_context);


    public IGenericRepository<ProjectStudent> ProjectStudents =>
        _projectStudents ??= new GenericRepository<ProjectStudent>(_context);


    public IGenericRepository<Transaction> Transactions =>
        _transactions ??= new GenericRepository<Transaction>(_context);


    public IGenericRepository<WeeklyReport> WeeklyReports =>
        _weeklyReports ??= new GenericRepository<WeeklyReport>(_context);
    
    public IGenericRepository<WeeklyReportFeedback> WeeklyReportFeedbacks =>
        _weeklyReportFeedbacks ??= new GenericRepository<WeeklyReportFeedback>(_context);

    public IGenericRepository<Term> Terms =>
        _terms ??= new GenericRepository<Term>(_context);

    public async Task<int> SaveChangesAsync(bool trackAudit = true, bool trackSoftDelete = true)
    {
        if (trackAudit)
        {
            TrackAuditChanges();
        }

        if (trackSoftDelete)
        {
            TrackSoftDeleteChanges();
        }

        return await _context.SaveChangesAsync();
    }

    private void TrackAuditChanges()
    {
        var entries = _context.ChangeTracker.Entries()
            .Where(e => e.Entity is AuditableEntity<Guid> or AuditableEntity<int> &&
                        e.State is EntityState.Added or EntityState.Modified);

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    switch (entry.Entity)
                    {
                        case AuditableEntity<Guid> guidEntity:
                            guidEntity.Id = Guid.NewGuid();
                            guidEntity.CreatedAt = DateTime.UtcNow;
                            break;
                        case AuditableEntity<int> intEntity:
                            intEntity.CreatedAt = DateTime.UtcNow;
                            break;
                    }

                    break;

                case EntityState.Modified:
                    switch (entry.Entity)
                    {
                        case AuditableEntity<Guid> guidEntity:
                            guidEntity.UpdatedAt = DateTime.UtcNow;
                            entry.Property(nameof(AuditableEntity<Guid>.CreatedAt)).IsModified = false;
                            break;
                        case AuditableEntity<int> intEntity:
                            intEntity.UpdatedAt = DateTime.UtcNow;
                            entry.Property(nameof(AuditableEntity<int>.CreatedAt)).IsModified = false;
                            break;
                    }

                    break;
            }
        }
    }

    private void TrackSoftDeleteChanges()
    {
        var entries = _context.ChangeTracker.Entries()
            .Where(e => e is { Entity: ISoftDeletable, State: EntityState.Deleted });

        foreach (var entry in entries)
        {
            entry.State = EntityState.Modified;
            if (entry.Entity is not ISoftDeletable softDeletable) continue;
            softDeletable.IsDeleted = true;
            softDeletable.DeletedAt = DateTime.UtcNow;
        }
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}