using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Repositories.Entities;
using Repositories.Utils;
using Services.Models.Request.Base;
using Services.Utils;

namespace Services.Models.Request.MentorAvailability;

public class CreateMentorAvailabilityRequest : ValidatableObject
{
    [JsonIgnore] public Guid MentorId { get; set; }
    public DateTime Date { get; set; }
    public IEnumerable<AvailableTimeSlot> AvailableTimeSlots { get; set; } = [];
    public byte[] TimeMap => AvailableTimeSlots.ToTimeMap();

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Date.Date <= DateTime.Today)
            yield return new ValidationResult("Date must be at least 1 day in advance");

        if (!TimeMap.IsTimeMapValid(Constants.MinAppointmentLength))
            yield return
                new ValidationResult($"Each session must be at least {Constants.MinAppointmentLength} minutes");
    }
}