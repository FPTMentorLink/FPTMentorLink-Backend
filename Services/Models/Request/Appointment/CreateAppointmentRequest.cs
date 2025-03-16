using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Services.Models.Request.Base;
using Services.Utils;

namespace Services.Models.Request.Appointment;

public class CreateAppointmentRequest : ValidatableObject
{
    public Guid ProjectId { get; set; }
    public Guid MentorId { get; set; }
    [JsonIgnore] public Guid LeaderId { get; set; } // Will be set using principal in the controller
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    /// <summary>
    /// Total time in minutes
    /// </summary>
    public int TotalMinutes => (int)(EndTime - StartTime).TotalMinutes;

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartTime < DateTime.Now.AddDays(1))
            yield return Helper.CreateValidationResult("Start time must be greater than current time");
        if (EndTime <= StartTime)
            yield return Helper.CreateValidationResult("End time must be greater than start time");
        if (StartTime.Date != EndTime.Date)
            yield return Helper.CreateValidationResult("Appointment must be on the same day");
        // Check if appointment is at least 48 hours in advance
        if (DateTime.UtcNow.AddHours(Constants.RequireCreateAppointmentInAdvance) > StartTime)
            yield return Helper.CreateValidationResult(
                $"Appointment must be at least {Constants.RequireCreateAppointmentInAdvance} hours in advance");
        if (TotalMinutes < Constants.MinAppointmentLength)
            yield return Helper.CreateValidationResult(
                $"Appointment must be at least {Constants.MinAppointmentLength} minutes long");
    }
}