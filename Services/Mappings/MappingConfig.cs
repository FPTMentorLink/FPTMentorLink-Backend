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
using Services.Models.Response.Account;
using Services.Models.Response.Appointment;
using Services.Models.Response.Authentication;
using Services.Models.Response.Checkpoint;
using Services.Models.Response.CheckpointTask;
using Services.Models.Response.Feedback;
using Services.Models.Response.Lecturer;
using Services.Models.Response.Mentor;

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
        TypeAdapterConfig<Project, ProjectDto>.NewConfig();
        TypeAdapterConfig<CreateProjectDto, Project>.NewConfig();
        TypeAdapterConfig<UpdateProjectDto, Project>.NewConfig()
            .IgnoreNullValues(true);

        // Proposal
        TypeAdapterConfig<Proposal, ProposalDto>.NewConfig();
        TypeAdapterConfig<CreateProposalDto, Proposal>.NewConfig();
        TypeAdapterConfig<UpdateProposalDto, Proposal>.NewConfig()
            .IgnoreNullValues(true);

        // Student
        TypeAdapterConfig<Student, StudentDto>.NewConfig();
        TypeAdapterConfig<CreateStudentDto, Student>.NewConfig();
        TypeAdapterConfig<UpdateStudentDto, Student>.NewConfig()
            .IgnoreNullValues(true);

        // TODO: Add mapping config for term


        // ProjectStudent
        TypeAdapterConfig<ProjectStudent, ProjectStudentDto>.NewConfig();
        TypeAdapterConfig<CreateProjectStudentDto, ProjectStudent>.NewConfig();
        TypeAdapterConfig<UpdateProjectStudentDto, ProjectStudent>.NewConfig()
            .IgnoreNullValues(true);

        // WeeklyReport
        TypeAdapterConfig<WeeklyReport, WeeklyReportDto>.NewConfig();
        TypeAdapterConfig<CreateWeeklyReportDto, WeeklyReport>.NewConfig();
        TypeAdapterConfig<UpdateWeeklyReportDto, WeeklyReport>.NewConfig()
            .IgnoreNullValues(true);
    }
}