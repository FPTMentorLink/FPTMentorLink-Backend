using Repositories.Entities;

namespace Services.DTOs;

public class AccountDto
{
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsSuspended { get; set; }
    public AccountRole Role { get; set; }
}

public class CreateAccountDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsSuspended { get; set; }
    public AccountRole Role { get; set; }
}

public class UpdateAccountDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ImageUrl { get; set; }
    public bool? IsSuspended { get; set; }
}

public class LoginRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class LoginResponse
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? ImageUrl { get; set; }
    public AccountRole Role { get; set; }
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}

public class RefreshTokenRequest
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}

public class TokenResponse
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}