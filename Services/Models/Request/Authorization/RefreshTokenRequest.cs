using System.Text.Json.Serialization;

namespace Services.Models.Request.Authorization;

public class RefreshTokenRequest
{
    [JsonIgnore] public string AccessToken { get; set; } = null!;
    public required string RefreshToken { get; set; } 
}