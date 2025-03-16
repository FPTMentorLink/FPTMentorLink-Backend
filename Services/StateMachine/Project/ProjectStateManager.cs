using Repositories.Entities;
using Services.Utils;

namespace Services.StateMachine.Project;

/// <summary>
/// Manages project state transitions and validations.
/// All transitions are controlled by lecturers.
/// </summary>
public static class ProjectStateManager
{
    /// <summary>
    /// Defines valid transitions from each project status
    /// </summary>
    private static readonly Dictionary<ProjectStatus, HashSet<ProjectStatus>> AllowedTransitions = new()
    {
        // From Pending: Can move to InProgress (when requirements met) or directly to Closed
        {
            ProjectStatus.Pending, [
                ProjectStatus.InProgress,
                ProjectStatus.Closed
            ]
        },
        
        // From InProgress: Can be marked for revision, completed, failed, pending review, or closed
        {
            ProjectStatus.InProgress, [
                ProjectStatus.RevisionRequired,
                ProjectStatus.Completed,
                ProjectStatus.Failed,
                ProjectStatus.PendingReview,
                ProjectStatus.Closed
            ]
        },
        
        // From PendingReview: Can be revised, completed, failed, or closed
        {
            ProjectStatus.PendingReview, [
                ProjectStatus.RevisionRequired,
                ProjectStatus.Completed,
                ProjectStatus.Failed,
                ProjectStatus.Closed
            ]
        },
        
        // From RevisionRequired: Can be completed, failed, or closed
        {
            ProjectStatus.RevisionRequired, [
                ProjectStatus.Completed,
                ProjectStatus.Failed,
                ProjectStatus.Closed
            ]
        },
        
        // From Completed/Failed: Can only be closed
        {
            ProjectStatus.Completed, [ProjectStatus.Closed]
        },
        {
            ProjectStatus.Failed, [ProjectStatus.Closed]
        }
        
        // Closed status is terminal - no transitions allowed
    };

    /// <summary>
    /// Defines which roles can perform transitions to specific statuses
    /// </summary>
    private static readonly Dictionary<ProjectStatus, HashSet<AccountRole>> AllowedRoles = new()
    {
        { ProjectStatus.InProgress, [AccountRole.Lecturer] },
        { ProjectStatus.PendingReview, [AccountRole.Lecturer] },
        { ProjectStatus.RevisionRequired, [AccountRole.Lecturer] },
        { ProjectStatus.Completed, [AccountRole.Lecturer] },
        { ProjectStatus.Failed, [AccountRole.Lecturer] },
        { ProjectStatus.Closed, [AccountRole.Lecturer, AccountRole.Admin] }
    };

    /// <summary>
    /// Validates if the requested status transition is allowed and if the user role has permission
    /// </summary>
    public static Result ValidateTransition(ProjectStatus currentStatus, ProjectStatus nextStatus, AccountRole userRole)
    {
        // Check if project is closed
        if (currentStatus == ProjectStatus.Closed)
            return Result.Failure("Closed projects cannot be updated");
            
        // Check if transition is valid
        if (!AllowedTransitions.TryGetValue(currentStatus, out var validNextStatuses))
            return Result.Failure($"Invalid current status: {currentStatus}");
            
        if (!validNextStatuses.Contains(nextStatus))
            return Result.Failure($"Cannot transition from {currentStatus} to {nextStatus}");
            
        // Check if role has permission
        if (!AllowedRoles.TryGetValue(nextStatus, out var allowedRoles))
            return Result.Failure($"No roles are allowed to transition to {nextStatus}");

        if (!allowedRoles.Contains(userRole))
            return Result.Failure($"{userRole} is not allowed to transition to {nextStatus}");

        return Result.Success();
    }
}
