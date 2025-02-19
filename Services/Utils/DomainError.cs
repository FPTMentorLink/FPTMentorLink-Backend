namespace Services.Utils;

public static class DomainError
{
    private static class CommonError
    {
        public static string NotFound(string name)
        {
            return $"{name} not found.";
        }

        public static string Exists(string name)
        {
            return $"{name} already exists.";
        }
    }

    public static class Account
    {
        public static readonly string AccountNotFound = CommonError.NotFound(nameof(Repositories.Entities.Account));
        public static readonly string InvalidFptEmail = "Email must be an FPT email.";
    }
}