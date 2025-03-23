using System.ComponentModel.DataAnnotations;
using Services.Models.Request.Base;

namespace Services.Models.Request.ProjectStudent;

public class SendProjectInvitationRequest : ValidatableObject
{
    [Required] public required string Email { get; set; }
    [Required] public Guid ProjectId { get; set; }
}