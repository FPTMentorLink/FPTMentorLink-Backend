using System.Text.Json.Serialization;
using Repositories.Entities;

namespace Services.Models.Response.Authentication;

public class LoginResponse
{
    [JsonPropertyName("id")] public Guid Id { get; set; }

    [JsonPropertyName("email")] public string Email { get; set; }

    [JsonPropertyName("firstName")] public string FirstName { get; set; }

    [JsonPropertyName("lastName")] public string LastName { get; set; }

    [JsonPropertyName("imageUrl")] public string? ImageUrl { get; set; }

    [JsonPropertyName("role")] public int Role { get; set; }

    [JsonPropertyName("accessToken")] public string AccessToken { get; set; }

    [JsonPropertyName("refreshToken")] public string RefreshToken { get; set; }
}