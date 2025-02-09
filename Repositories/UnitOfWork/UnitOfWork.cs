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
    private IGenericRepository<Checkpoint>? _checkpoints;
    private IGenericRepository<CheckpointTask>? _checkpointTasks;
    private IGenericRepository<Feedback>? _feedbacks;
    private IGenericRepository<Group>? _groups;
    private IGenericRepository<Lecturer>? _lecturers;
    private IGenericRepository<Mentor>? _mentors;
    private IGenericRepository<MentorAvailability>? _mentorAvailabilities;
    private IGenericRepository<Project>? _projects;
    private IGenericRepository<Proposal>? _proposals;
    private IGenericRepository<Student>? _students;
    private IGenericRepository<StudentGroup>? _studentGroups;
    private IGenericRepository<TaskLog>? _taskLogs;
    private IGenericRepository<Transactions>? _transactions;
    private IGenericRepository<WeeklyReports>? _weeklyReports;


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

    public IGenericRepository<Feedback> Feedbacks =>
        _feedbacks ??= new GenericRepository<Feedback>(_context);

    public IGenericRepository<Group> Groups =>
        _groups ??= new GenericRepository<Group>(_context);

    public IGenericRepository<Lecturer> Lecturers =>
        _lecturers ??= new GenericRepository<Lecturer>(_context);

    public IGenericRepository<Mentor> Mentors =>
        _mentors ??= new GenericRepository<Mentor>(_context);


    public IGenericRepository<MentorAvailability> MentorAvailabilities =>
        _mentorAvailabilities ??= new GenericRepository<MentorAvailability>(_context);


    public IGenericRepository<Project> Projects =>
        _projects ??= new GenericRepository<Project>(_context);


    public IGenericRepository<Proposal> Proposals =>
        _proposals ??= new GenericRepository<Proposal>(_context);


    public IGenericRepository<Student> Students =>
        _students ??= new GenericRepository<Student>(_context);


    public IGenericRepository<StudentGroup> StudentGroups =>
        _studentGroups ??= new GenericRepository<StudentGroup>(_context);


    public IGenericRepository<TaskLog> TaskLogs =>
        _taskLogs ??= new GenericRepository<TaskLog>(_context);


    public IGenericRepository<Transactions> Transactions =>
        _transactions ??= new GenericRepository<Transactions>(_context);


    public IGenericRepository<WeeklyReports> WeeklyReports =>
        _weeklyReports ??= new GenericRepository<WeeklyReports>(_context);

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
                            guidEntity.UpdatedAt = DateTime.UtcNow;
                            break;
                        case AuditableEntity<int> intEntity:
                            intEntity.CreatedAt = DateTime.UtcNow;
                            intEntity.UpdatedAt = DateTime.UtcNow;
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