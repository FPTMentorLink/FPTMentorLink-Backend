namespace Services.Utils.Hmac;

public class HashModel
{
    public string UserFullName { get; set; } = null!;
    public string UserEmail { get; set; } = null!;
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}