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
        public static readonly string DuplicateEmailCsv = "Duplicate emails found in CSV: ";
        public static readonly string DuplicateUsernameCsv = "Duplicate usernames found in CSV: ";
        public static readonly string MaxImportAccountsExceeded = "Maximum number of accounts to import is 300.";

        public static readonly string CsvAccountExist =
            "Accounts in csv already exist in the database. Please check again";

        public static readonly string EmailExists = CommonError.Exists(nameof(Repositories.Entities.Account.Email));

        public static readonly string UsernameExists =
            CommonError.Exists(nameof(Repositories.Entities.Account.Username));

        public static readonly string AccountNotFound = CommonError.NotFound(nameof(Repositories.Entities.Account));
        public static readonly string InvalidFptEmail = "Email must be an FPT email.";
    }

    public static class Lecturer
    {
        public static readonly string LecturerNotFound = CommonError.NotFound(nameof(Repositories.Entities.Lecturer));

        public static readonly string LecturerCodeExists =
            CommonError.Exists(nameof(Repositories.Entities.Lecturer.Code));

        public static readonly string DuplicateLecturerCodeCsv = "Duplicate lecturer codes found in CSV: ";
    }

    public static class Mentor
    {
        public static readonly string MentorNotFound = CommonError.NotFound(nameof(Repositories.Entities.Mentor));
        public static readonly string MentorCodeExists = CommonError.Exists(nameof(Repositories.Entities.Mentor.Code));
        public static readonly string DuplicateLecturerCodeCsv = "Duplicate mentor codes found in CSV: ";
    }

    public static class Student
    {
        public static readonly string StudentNotFound = CommonError.NotFound(nameof(Repositories.Entities.Student));

        public static readonly string
            StudentCodeExists = CommonError.Exists(nameof(Repositories.Entities.Student.Code));

        public static readonly string DuplicateStudentCodeCsv = "Duplicate student codes found in CSV: ";
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

        public static readonly string InvalidStatusTransition =
            "Invalid status transition. Cannot change to the previous status.";

        public static readonly string CheckpointTaskHasBeenCompleted =
            "Checkpoint task has been completed. Cannot update.";

        public static readonly string CheckpointTaskInProgress =
            "Checkpoint task is in progress of review. Cannot delete.";
    }

    public static class Project
    {
        public static readonly string ProjectNotFound = CommonError.NotFound(nameof(Repositories.Entities.Project));
    }

    public static class Faculty
    {
        public static readonly string FacultyNotFound = CommonError.NotFound(nameof(Repositories.Entities.Faculty));
    }
}