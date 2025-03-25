using Mapster;
using Repositories.Entities;
using Services.Models.Request.Account;
using Services.Models.Request.Appointment;
using Services.Models.Request.AppointmentFeedback;
using Services.Models.Request.Checkpoint;
using Services.Models.Request.CheckpointTask;
using Services.Models.Request.Faculty;
using Services.Models.Request.Lecturer;
using Services.Models.Request.LecturingProposal;
using Services.Models.Request.Mentor;
using Services.Models.Request.MentorAvailability;
using Services.Models.Request.MentoringProposal;
using Services.Models.Request.Notification;
using Services.Models.Request.Project;
using Services.Models.Request.ProjectStudent;
using Services.Models.Request.Proposal;
using Services.Models.Request.Student;
using Services.Models.Request.Term;
using Services.Models.Request.WeeklyReport;
using Services.Models.Response.Account;
using Services.Models.Response.Appointment;
using Services.Models.Response.AppointmentFeedback;
using Services.Models.Response.Authentication;
using Services.Models.Response.Checkpoint;
using Services.Models.Response.CheckpointTask;
using Services.Models.Response.Faculty;
using Services.Models.Response.Lecturer;
using Services.Models.Response.LecturingProposal;
using Services.Models.Response.Mentor;
using Services.Models.Response.MentorAvailability;
using Services.Models.Response.MentoringProposal;
using Services.Models.Response.Notification;
using Services.Models.Response.Project;
using Services.Models.Response.ProjectStudent;
using Services.Models.Response.Proposal;
using Services.Models.Response.Student;
using Services.Models.Response.Term;
using Services.Models.Response.Transaction;
using Services.Models.Response.WeeklyReport;
using Services.Models.Response.WeeklyReportFeedback;

namespace Services.Mappings;

public static class MappingConfig
{
    public static void RegisterMappings()
    {
        // Appointment
        TypeAdapterConfig<Appointment, AppointmentResponse>.NewConfig()
            .Map(dest => dest.ProjectName, src => src.Project.Name)
            .Map(dest => dest.MentorName, src => src.Mentor.Account.FirstName + " " + src.Mentor.Account.LastName);
        TypeAdapterConfig<CreateAppointmentRequest, Appointment>.NewConfig();
        TypeAdapterConfig<UpdateAppointmentStatusRequest, Appointment>.NewConfig()
            .IgnoreNullValues(true);

        // Account
        TypeAdapterConfig<Account, AccountResponse>.NewConfig()
            .Map(x => x.RoleName, src => src.Role.ToString());
        TypeAdapterConfig<BaseCreateAccountRequest, Account>.NewConfig()
            .Map(x => x.PasswordHash, src => src.Password);
        TypeAdapterConfig<Account, LoginResponse>.NewConfig();
        TypeAdapterConfig<BaseUpdateAccountRequest, Account>.NewConfig()
            .IgnoreNullValues(true);
        TypeAdapterConfig<CsvAccount, Account>.NewConfig()
            .Map(x => x.PasswordHash, src => src.Password);
        TypeAdapterConfig<Account, AdminResponse>.NewConfig();


        // Checkpoint
        TypeAdapterConfig<Checkpoint, CheckpointResponse>.NewConfig();
        TypeAdapterConfig<CreateCheckpointRequest, Checkpoint>.NewConfig();
        TypeAdapterConfig<UpdateCheckpointRequest, Checkpoint>.NewConfig()
            .IgnoreNullValues(true);

        // CheckpointTask
        TypeAdapterConfig<CheckpointTask, CheckpointTaskResponse>.NewConfig()
            .Map(dest => dest.CheckpointName, src => src.Checkpoint.Name)
            .Map(dest => dest.ProjectName, src => src.Project.Name);
        TypeAdapterConfig<CreateCheckpointTaskRequest, CheckpointTask>.NewConfig();
        TypeAdapterConfig<UpdateCheckpointTaskRequest, CheckpointTask>.NewConfig()
            .IgnoreNullValues(true);

        // Faculty
        TypeAdapterConfig<Faculty, FacultyResponse>.NewConfig();
        TypeAdapterConfig<CreateFacultyRequest, Faculty>.NewConfig();
        TypeAdapterConfig<UpdateFacultyRequest, Faculty>.NewConfig()
            .IgnoreNullValues(true);

        // Appointment Feedback
        TypeAdapterConfig<AppointmentFeedback, AppointmentFeedbackResponse>.NewConfig();
        TypeAdapterConfig<CreateAppointmentFeedbackRequest, AppointmentFeedback>.NewConfig();
        TypeAdapterConfig<UpdateAppointmentFeedbackRequest, AppointmentFeedback>.NewConfig()
            .IgnoreNullValues(true);

        // Lecturer
        TypeAdapterConfig<Lecturer, LecturerResponse>.NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Email, src => src.Account.Email)
            .Map(dest => dest.Username, src => src.Account.Username)
            .Map(dest => dest.FirstName, src => src.Account.FirstName)
            .Map(dest => dest.LastName, src => src.Account.LastName)
            .Map(dest => dest.ImageUrl, src => src.Account.ImageUrl)
            .Map(dest => dest.IsSuspended, src => src.Account.IsSuspended)
            .Map(dest => dest.Role, src => src.Account.Role)
            .Map(dest => dest.Faculty, src => src.Faculty.Name);
        TypeAdapterConfig<CreateLecturerRequest, Lecturer>.NewConfig();
        TypeAdapterConfig<UpdateLecturerRequest, Lecturer>.NewConfig()
            .IgnoreNullValues(true);

        // Transaction
        TypeAdapterConfig<Transaction, TransactionResponse>.NewConfig()
            .Map(dest => dest.TransactionCode, src => src.Code)
            .Map(dest => dest.FullName, src => src.Account.FirstName + " " + src.Account.LastName)
            .Map(dest => dest.Role, src => src.Account.Role.ToString());

        // LecturingProposal
        TypeAdapterConfig<LecturingProposal, LecturingProposalResponse>.NewConfig();
        TypeAdapterConfig<CreateLecturingProposalRequest, LecturingProposal>.NewConfig();
        TypeAdapterConfig<StudentUpdateLecturingProposalRequest, LecturingProposal>.NewConfig()
            .IgnoreNullValues(true);
        TypeAdapterConfig<LecturerUpdateLecturingProposalRequest, LecturingProposal>.NewConfig()
            .IgnoreNullValues(true);

        // Mentor
        TypeAdapterConfig<Account, MentorResponse>.NewConfig();
        TypeAdapterConfig<Mentor, MentorResponse>.NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Code, src => src.Code)
            .Map(dest => dest.Email, src => src.Account.Email)
            .Map(dest => dest.Username, src => src.Account.Username)
            .Map(dest => dest.FirstName, src => src.Account.FirstName)
            .Map(dest => dest.LastName, src => src.Account.LastName)
            .Map(dest => dest.ImageUrl, src => src.Account.ImageUrl)
            .Map(dest => dest.IsSuspended, src => src.Account.IsSuspended)
            .Map(dest => dest.Role, src => src.Account.Role);
        TypeAdapterConfig<CreateMentorRequest, Mentor>.NewConfig();
        TypeAdapterConfig<UpdateMentorRequest, Mentor>.NewConfig()
            .IgnoreNullValues(true);

        // MentorAvailability
        TypeAdapterConfig<MentorAvailability, MentorAvailabilityResponse>.NewConfig()
            .Map(dest => dest.AvailableTimeSlots, src => src.GetAvailableTimeSlots().ToList());
        TypeAdapterConfig<CreateMentorAvailabilityRequest, MentorAvailability>.NewConfig();
        TypeAdapterConfig<UpdateMentorAvailabilityRequest, MentorAvailability>.NewConfig()
            .IgnoreNullValues(true);

        // MentoringProposal
        TypeAdapterConfig<MentoringProposal, MentoringProposalResponse>.NewConfig()
            .Map(dest => dest.ProjectName, src => src.Project != null ? src.Project.Name : null)
            .Map(dest => dest.MentorName, src => src.Mentor != null && src.Mentor.Account != null ? 
                $"{src.Mentor.Account.FirstName} {src.Mentor.Account.LastName}" : null);
        
        TypeAdapterConfig<CreateMentoringProposalRequest, MentoringProposal>.NewConfig();
        TypeAdapterConfig<StudentUpdateMentoringProposalRequest, MentoringProposal>.NewConfig()
            .IgnoreNullValues(true);
        TypeAdapterConfig<MentorUpdateMentoringProposalRequest, MentoringProposal>.NewConfig()
            .IgnoreNullValues(true);

        // Project
        TypeAdapterConfig<Project, ProjectDetailResponse>.NewConfig()
            .Map(dest => dest.TermCode, src => src.Term.Code)
            .Map(dest => dest.FacultyCode, src => src.Faculty.Code);
        TypeAdapterConfig<Project, ProjectResponse>.NewConfig()
            .Map(dest => dest.TermCode, src => src.Term.Code)
            .Map(dest => dest.FacultyCode, src => src.Faculty.Code);
        TypeAdapterConfig<CreateProjectRequest, Project>.NewConfig();
        TypeAdapterConfig<UpdateProjectRequest, Project>.NewConfig()
            .IgnoreNullValues(true);

        // Proposal
        TypeAdapterConfig<Proposal, ProposalResponse>.NewConfig();
        TypeAdapterConfig<CreateProposalRequest, Proposal>.NewConfig();
        TypeAdapterConfig<UpdateProposalRequest, Proposal>.NewConfig()
            .IgnoreNullValues(true);

        // Student
        TypeAdapterConfig<Student, StudentResponse>.NewConfig()
            // Map other Student-specific properties
            // Map Account properties
            .Map(dest => dest.Email, src => src.Account.Email)
            .Map(dest => dest.Username, src => src.Account.Username)
            .Map(dest => dest.FirstName, src => src.Account.FirstName)
            .Map(dest => dest.LastName, src => src.Account.LastName)
            .Map(dest => dest.ImageUrl, src => src.Account.ImageUrl)
            .Map(dest => dest.IsSuspended, src => src.Account.IsSuspended)
            .Map(dest => dest.Role, src => src.Account.Role)
            .Map(dest => dest.Faculty, src => src.Faculty.Name);
        TypeAdapterConfig<CreateStudentRequest, Student>.NewConfig();
        TypeAdapterConfig<UpdateStudentRequest, Student>.NewConfig()
            .IgnoreNullValues(true);

        // TODO: Add mapping config for term


        // ProjectStudent
        TypeAdapterConfig<ProjectStudent, ProjectStudentResponse>.NewConfig()
            .Map(dest => dest.Code, src => src.Student.Code)
            .Map(dest => dest.FirstName, src => src.Student.Account.FirstName)
            .Map(dest => dest.LastName, src => src.Student.Account.LastName);
        TypeAdapterConfig<CreateProjectStudentRequest, ProjectStudent>.NewConfig();

        // WeeklyReport
        TypeAdapterConfig<WeeklyReport, WeeklyReportResponse>.NewConfig();
        TypeAdapterConfig<CreateWeeklyReportRequest, WeeklyReport>.NewConfig();
        TypeAdapterConfig<UpdateWeeklyReportRequest, WeeklyReport>.NewConfig()
            .IgnoreNullValues(true);

        TypeAdapterConfig<WeeklyReport, WeeklyReportDetailResponse>.NewConfig();

        // WeeklyReportFeedback
        TypeAdapterConfig<WeeklyReportFeedback, WeeklyReportFeedBackResponse>.NewConfig()
            .Map(dest => dest.LecturerName,
                src => src.Lecturer.Account.FirstName + " " + src.Lecturer.Account.LastName + " (" +
                       src.Lecturer.Account.Email + ")");

        // Term
        TypeAdapterConfig<Term, TermResponse>.NewConfig();
        TypeAdapterConfig<CreateTermRequest, Term>.NewConfig();
        TypeAdapterConfig<UpdateTermRequest, Term>.NewConfig()
            .IgnoreNullValues(true);

        // Notification mappings
        TypeAdapterConfig<Notification, NotificationResponse>.NewConfig();
        TypeAdapterConfig<CreateNotificationRequest, Notification>.NewConfig();
    }
}