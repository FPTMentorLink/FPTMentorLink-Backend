using Mapster;
using Repositories.Entities;
using Services.DTOs;
using Services.Models.Request.Account;
using Services.Models.Request.Appointment;
using Services.Models.Response.Account;
using Services.Models.Response.Appointment;
using Services.Models.Response.Authentication;

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
        TypeAdapterConfig<Checkpoint, CheckpointDto>.NewConfig();
        TypeAdapterConfig<CreateCheckpointDto, Checkpoint>.NewConfig();
        TypeAdapterConfig<UpdateCheckpointDto, Checkpoint>.NewConfig()
            .IgnoreNullValues(true);

        // CheckpointTask
        TypeAdapterConfig<CheckpointTask, CheckpointTaskDto>.NewConfig();
        TypeAdapterConfig<CreateCheckpointTaskDto, CheckpointTask>.NewConfig();
        TypeAdapterConfig<UpdateCheckpointTaskDto, CheckpointTask>.NewConfig()
            .IgnoreNullValues(true);

        // Feedback
        TypeAdapterConfig<Feedback, FeedbackDto>.NewConfig();
        TypeAdapterConfig<CreateFeedbackDto, Feedback>.NewConfig();
        TypeAdapterConfig<UpdateFeedbackDto, Feedback>.NewConfig()
            .IgnoreNullValues(true);

        // Lecturer
        TypeAdapterConfig<Lecturer, LecturerDto>.NewConfig();
        TypeAdapterConfig<CreateLecturerDto, Lecturer>.NewConfig();
        TypeAdapterConfig<UpdateLecturerDto, Lecturer>.NewConfig()
            .IgnoreNullValues(true);

        // Mentor
        TypeAdapterConfig<Mentor, MentorDto>.NewConfig();
        TypeAdapterConfig<CreateMentorDto, Mentor>.NewConfig();
        TypeAdapterConfig<UpdateMentorDto, Mentor>.NewConfig()
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