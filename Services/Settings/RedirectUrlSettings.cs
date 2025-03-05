namespace Services.Settings;

public class RedirectUrlSettings
{
    public string PostGoogleLoginUrl { get; set; } = null!;
    public string LoginFailedUrl { get; set; } = null!;
    public string Key { get; set; } = null!;
    public int TimeOut { get; set; }
}