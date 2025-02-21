namespace Services.Models.Request.Account;

public class UpdateAccountRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ImageUrl { get; set; }
    public bool? IsSuspended { get; set; }
}