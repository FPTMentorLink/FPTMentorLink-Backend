namespace Services.Utils;

public static class CodeGenerator
{
    private static string GenerateSequence(int number, int length = 3)
    {
        return number.ToString($"D{length}");
    }
    private static string GenerateCode(string prefix, int number, int length = 3)
    {
        return $"{prefix}{GenerateSequence(number, length)}";
    }

    public static string GenerateProjectCode(string term, string faculty, int number)
    {
        return GenerateCode($"{term}{faculty}", number, 6);
    }

}