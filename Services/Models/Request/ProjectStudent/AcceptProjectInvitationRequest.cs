using System.ComponentModel.DataAnnotations;
using Services.Models.Request.Base;

namespace Services.Models.Request.ProjectStudent;

public class AcceptProjectInvitationRequest : ValidatableObject
{
    [Required] public string Token { get; set; } = null!;
}