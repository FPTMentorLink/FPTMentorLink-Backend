namespace Services.Models.Request.Authorization;

public class RefreshTokenRequest
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}