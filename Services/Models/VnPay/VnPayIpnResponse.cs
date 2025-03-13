using System.Text.Json.Serialization;

namespace Services.Models.VnPay;

public class VnPayIpnResponse
{
    [JsonPropertyName("RspCode")] public string RspCode { get; set; }
    [JsonPropertyName("Message")] public string Message { get; set; }
}