namespace Services.Models.Request.Authentication;

public class LoginRequest
{
    public Guid UserId { get; set; }
    public required string Token { get; set; }
}