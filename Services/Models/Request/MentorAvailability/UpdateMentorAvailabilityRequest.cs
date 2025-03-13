using System.ComponentModel.DataAnnotations;
using Repositories.Entities;
using Repositories.Utils;
using Services.Models.Request.Base;
using Services.Utils;

namespace Services.Models.Request.MentorAvailability;

public class UpdateMentorAvailabilityRequest : ValidatableObject
{
    public IEnumerable<AvailableTimeSlot> AvailableTimeSlots { get; set; } = [];
    public byte[] TimeMap => AvailableTimeSlots.ToTimeMap();
    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext){
        if (!TimeMap.IsTimeMapValid(Constants.MinAppointmentSlotLength)){
            yield return new ValidationResult($"Each session must be at least {Constants.MinAppointmentSlotLength} minutes");
        }
    }
}