namespace Services.Utils.Hmac;

public class RedirectUrlSettings
{
    public string FrontEndUrl { get; set; } = null!;
    public string Key { get; set; } = null!;
    public int TimeOut { get; set; }
}