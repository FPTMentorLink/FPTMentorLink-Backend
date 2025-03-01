using System.Security.Cryptography;
using System.Text;

namespace Services.Utils.Hmac;

public static class HmacHelper
{
    public static string GenerateSignature(string secretKey, string data)
    {
        var key = Encoding.UTF8.GetBytes(secretKey);
        var message = Encoding.UTF8.GetBytes(data);

        using var hmacsha256 = new HMACSHA256(key);
        var hashMessage = hmacsha256.ComputeHash(message);
        return Convert.ToBase64String(hashMessage);
    }
    
    public static string ComputeContentHash(string content)
    {
        using var sha256 = SHA256.Create();
        byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(content));
        return Convert.ToBase64String(hashedBytes);
    }
    
    // Optional: Validate if the timestamp is recent (e.g., within the last 5 minutes)
    public static bool IsTimestampValid(string timestamp, string lifeTime)
    {
        if (DateTime.TryParse(timestamp, out var parsedTimestamp))
        {
            var currentTime = DateTime.UtcNow;
            var timeDifference = currentTime - parsedTimestamp;
            return timeDifference.TotalMinutes < int.Parse(lifeTime);
        }
        return false;
    }
}