using Repositories.Entities;
using Services.Utils;

namespace Services.StateMachine.Appointment;

public static class AppointmentStateManager
{
    private static readonly Dictionary<AppointmentStatus, HashSet<AppointmentStatus>> AllowedTransitions = new()
    {
        // From Pending: Can be accepted, rejected, canceled, cancel requested
        {
            AppointmentStatus.Pending, [
                AppointmentStatus.Accepted,
                AppointmentStatus.Rejected,
                AppointmentStatus.Canceled,
                AppointmentStatus.CancelRequested // Student can request cancel if the mentor didn't accept the appointment after the end time
            ]
        },

        // From Accepted: Can be canceled, pending confirmation
        {
            AppointmentStatus.Accepted, [
                AppointmentStatus.Canceled,
                AppointmentStatus.PendingConfirmation
            ]
        },

        // From PendingConfirmation: Can be confirmed by student, confirmed by mentor, or cancel requested
        {
            AppointmentStatus.PendingConfirmation, [
                AppointmentStatus.ConfirmedByStudent,
                AppointmentStatus.ConfirmedByMentor,
                AppointmentStatus.CancelRequested,
                AppointmentStatus.Canceled
            ]
        },

        // From ConfirmedByStudent: Can be confirmed by mentor
        {
            AppointmentStatus.ConfirmedByStudent, [
                AppointmentStatus.ConfirmedByMentor,
                AppointmentStatus.Canceled
            ]
        },

        // From ConfirmedByMentor: Can be confirmed by student, cancel requested
        {
            AppointmentStatus.ConfirmedByMentor, [
                AppointmentStatus.ConfirmedByStudent,
                AppointmentStatus.CancelRequested,
                AppointmentStatus.Canceled
            ]
        },

        // From CancelRequested: Can be canceled, completed
        {
            AppointmentStatus.CancelRequested, [
                AppointmentStatus.Canceled,
                AppointmentStatus.Completed
            ]
        },
        {
            AppointmentStatus.Rejected, []
        },
        {
            AppointmentStatus.Canceled, []
        },
        {
            AppointmentStatus.Completed, []
        }
    };

    private static readonly Dictionary<AppointmentStatus, HashSet<AccountRole>> AllowedRoles = new()
    {
        {
            AppointmentStatus.Accepted, [
                AccountRole.Mentor
            ]
        },
        {
            AppointmentStatus.Rejected, [
                AccountRole.Mentor
            ]
        },
        {
            AppointmentStatus.ConfirmedByMentor, [
                AccountRole.Mentor
            ]
        },
        {
            AppointmentStatus.ConfirmedByStudent, [
                AccountRole.Student
            ]
        },
        {
            AppointmentStatus.Canceled, [
                AccountRole.Student,
                AccountRole.Mentor,
                AccountRole.Admin
            ]
        },
        // Only student can request cancel
        {
            AppointmentStatus.CancelRequested, [
                AccountRole.Student,
            ]
        },
        {
            AppointmentStatus.Completed, [
                AccountRole.Admin
            ]
        }
    };

    public static Result ValidateTransition(AppointmentStatus currentStatus, AppointmentStatus nextStatus,
        AccountRole userRole)
    {
        // Check if status is terminal
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