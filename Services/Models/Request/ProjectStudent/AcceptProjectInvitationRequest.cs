using System.ComponentModel.DataAnnotations;
using Services.Models.Request.Base;

namespace Services.Models.Request.ProjectStudent;

public class AcceptProjectInvitationRequest : ValidatorObject
{
    [Required] public string Token { get; set; } = null!;
}