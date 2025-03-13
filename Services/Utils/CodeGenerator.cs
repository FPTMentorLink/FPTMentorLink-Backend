namespace Services.Utils;

public static class CodeGenerator
{
    private static string GenerateSequence(int number, int length = 3)
    {
        return number.ToString($"D{length}");
    }

    /// <summary>
    /// Generate a code based on the prefix and number
    /// </summary>
    /// <param name="prefix"></param>
    /// <param name="number"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    private static string GenerateCode(string prefix, int number, int length = 3)
    {
        return $"{prefix}{GenerateSequence(number, length)}";
    }

    /// <summary>
    /// Generate a project code based on the term, faculty, and number
    /// </summary>
    /// <param name="term"></param>
    /// <param name="faculty"></param>
    /// <param name="number"></param>
    /// <returns></returns>
    public static string GenerateProjectCode(string term, string faculty, int number)
    {
        return GenerateCode($"{term}{faculty}", number, 6);
    }
    
    /// <summary>
    /// Generate a random number with a specific length (default: 6)
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static long GenerateRandomNumber(int length = 6)
    {
        return new Random().Next((int)Math.Pow(10, length - 1), (int)Math.Pow(10, length) - 1);
    }

    /// <summary>
    /// Generate a transaction code based on the student code, VnPay transaction Id, and amount
    /// Ex: SE123456-123456-100000
    /// </summary>
    /// <param name="studentCode"></param>
    /// <param name="vnPayTransactionId"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public static string GenerateTransactionCode(string studentCode, long vnPayTransactionId, int amount)
    {
        return $"{studentCode}-{vnPayTransactionId}-{amount}";
    }
}