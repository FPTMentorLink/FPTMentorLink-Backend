using System.ComponentModel.DataAnnotations;
using Services.Models.Request.Base;

namespace Services.Models.Request.MentorFeedback;

public class UpdateMentorFeedbackRequest : ValidatableObject
{
    [MaxLength(2000)] public string? Content { get; set; } = null!;

    [Range(1, 5)] // Assuming rating is from 1 to 5
    public int? Rate { get; set; }
}