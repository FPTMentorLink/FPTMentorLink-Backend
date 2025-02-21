using Mapster;
using Repositories.Entities;
using Services.DTOs;
using Services.Models.Request.Account;
using Services.Models.Request.Appointment;
using Services.Models.Request.Checkpoint;
using Services.Models.Request.CheckpointTask;
using Services.Models.Request.Feedback;
using Services.Models.Request.Lecturer;
using Services.Models.Request.Mentor;
using Services.Models.Request.Project;
using Services.Models.Request.ProjectStudent;
using Services.Models.Request.Proposal;
using Services.Models.Request.Student;
using Services.Models.Request.WeeklyReport;
using Services.Models.Response.Account;
using Services.Models.Response.Appointment;
using Services.Models.Response.Authentication;
using Services.Models.Response.Checkpoint;
using Services.Models.Response.CheckpointTask;
using Services.Models.Response.Feedback;
using Services.Models.Response.Lecturer;
using Services.Models.Response.Mentor;
using Services.Models.Response.Project;
using Services.Models.Response.ProjectStudent;
using Services.Models.Response.Proposal;
using Services.Models.Response.Student;
using Services.Models.Response.WeeklyReport;

namespace Services.Mappings;

public static class MappingConfig
{
    public static void RegisterMappings()
    {
        // Appointment
        TypeAdapterConfig<Appointment, AppointmentResponse>.NewConfig();
        TypeAdapterConfig<CreateAppointmentRequest, Appointment>.NewConfig();
        TypeAdapterConfig<UpdateAppointmentRequest, Appointment>.NewConfig()
            .IgnoreNullValues(true);
        TypeAdapterConfig<UpdateAppointmentStatusRequest, Appointment>.NewConfig()
            .IgnoreNullValues(true);

        // Account
        TypeAdapterConfig<Account, AccountResponse>.NewConfig();
        TypeAdapterConfig<CreateAccountRequest, Account>.NewConfig();
        TypeAdapterConfig<Account, LoginResponse>.NewConfig();
        TypeAdapterConfig<UpdateAccountRequest, Account>.NewConfig()
            .IgnoreNullValues(true);

        // Checkpoint
        TypeAdapterConfig<Checkpoint, CheckpointResponse>.NewConfig();
        TypeAdapterConfig<CreateCheckpointRequest, Checkpoint>.NewConfig();
        TypeAdapterConfig<UpdateCheckpointRequest, Checkpoint>.NewConfig()
            .IgnoreNullValues(true);

        // CheckpointTask
        TypeAdapterConfig<CheckpointTask, CheckpointTaskResponse>.NewConfig();
        TypeAdapterConfig<CreateCheckpointTaskRequest, CheckpointTask>.NewConfig();
        TypeAdapterConfig<UpdateCheckpointTaskRequest, CheckpointTask>.NewConfig()
            .IgnoreNullValues(true);

        // Feedback
        TypeAdapterConfig<Feedback, FeedbackResponse>.NewConfig();
        TypeAdapterConfig<CreateFeedbackRequest, Feedback>.NewConfig();
        TypeAdapterConfig<UpdateFeedbackRequest, Feedback>.NewConfig()
            .IgnoreNullValues(true);

        // Lecturer
        TypeAdapterConfig<Lecturer, LecturerResponse>.NewConfig();
        TypeAdapterConfig<CreateLecturerRequest, Lecturer>.NewConfig();
        TypeAdapterConfig<UpdateLecturerRequest, Lecturer>.NewConfig()
            .IgnoreNullValues(true);

        // Mentor
        TypeAdapterConfig<Mentor, MentorResponse>.NewConfig();
        TypeAdapterConfig<CreateMentorRequest, Mentor>.NewConfig();
        TypeAdapterConfig<UpdateMentorRequest, Mentor>.NewConfig()
            .IgnoreNullValues(true);

        // MentorAvailability
        TypeAdapterConfig<MentorAvailability, MentorAvailabilityDto>.NewConfig();
        TypeAdapterConfig<CreateMentorAvailabilityDto, MentorAvailability>.NewConfig();
        TypeAdapterConfig<UpdateMentorAvailabilityDto, MentorAvailability>.NewConfig()
            .IgnoreNullValues(true);

        // Project
        TypeAdapterConfig<Project, ProjectResponse>.NewConfig();
        TypeAdapterConfig<CreateProjectRequest, Project>.NewConfig();
        TypeAdapterConfig<UpdateProjectRequest, Project>.NewConfig()
            .IgnoreNullValues(true);

        // Proposal
        TypeAdapterConfig<Proposal, ProposalResponse>.NewConfig();
        TypeAdapterConfig<CreateProposalRequest, Proposal>.NewConfig();
        TypeAdapterConfig<UpdateProposalRequest, Proposal>.NewConfig()
            .IgnoreNullValues(true);

        // Student
        TypeAdapterConfig<Student, StudentResponse>.NewConfig();
        TypeAdapterConfig<CreateStudentRequest, Student>.NewConfig();
        TypeAdapterConfig<UpdateStudentRequest, Student>.NewConfig()
            .IgnoreNullValues(true);

        // TODO: Add mapping config for term


        // ProjectStudent
        TypeAdapterConfig<ProjectStudent, ProjectStudentResponse>.NewConfig();
        TypeAdapterConfig<CreateProjectStudentRequest, ProjectStudent>.NewConfig();
        TypeAdapterConfig<UpdateProjectStudentRequest, ProjectStudent>.NewConfig()
            .IgnoreNullValues(true);

        // WeeklyReport
        TypeAdapterConfig<WeeklyReport, WeeklyReportResponse>.NewConfig();
        TypeAdapterConfig<CreateWeeklyReportRequest, WeeklyReport>.NewConfig();
        TypeAdapterConfig<UpdateWeeklyReportRequest, WeeklyReport>.NewConfig()
            .IgnoreNullValues(true);
    }
}