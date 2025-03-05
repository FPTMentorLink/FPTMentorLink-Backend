using System.ComponentModel.DataAnnotations;
using Services.Models.Request.Base;

namespace Services.Models.Request.MentorFeedback;

public class CreateMentorFeedbackRequest : ValidatableObject
{
    [Required] public Guid MentorId { get; set; }

    [Required] public Guid StudentId { get; set; }

    [Required] [MaxLength(2000)] public string? Content { get; set; }

    [Required]
    [Range(1, 5)] // Assuming rating is from 1 to 5
    public int Rate { get; set; }
}