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

        public static string InvalidTime()
        {
            return "Start time must be before end time.";
        }
    }

    public static class Account
    {
        public static readonly string AccountNotFound = CommonError.NotFound(nameof(Repositories.Entities.Account));
        public static readonly string InvalidFptEmail = "Email must be an FPT email.";
    }

    public static class Term
    {
        public static readonly string TermNotFound = CommonError.NotFound(nameof(Repositories.Entities.Term));
        public static readonly string TermCodeExists = CommonError.Exists(nameof(Repositories.Entities.Term.Code));
        public static readonly string InvalidTime = CommonError.InvalidTime();
    }

    public static class Checkpoint
    {
        public static readonly string CheckpointNotFound =
            CommonError.NotFound(nameof(Repositories.Entities.Checkpoint));

        public static readonly string
            ExceedCheckpoint = "Maximum number of checkpoints for this term has been reached. (Maximum 4 checkpoints)";

        public static readonly string InvalidTime = CommonError.InvalidTime();
    }

    public static class CheckpointTask
    {
        public static readonly string CheckpointTaskNotFound =
            CommonError.NotFound(nameof(Repositories.Entities.CheckpointTask));
    }

    public static class Project
    {
        public static readonly string ProjectNotFound = CommonError.NotFound(nameof(Repositories.Entities.Project));
    }
}