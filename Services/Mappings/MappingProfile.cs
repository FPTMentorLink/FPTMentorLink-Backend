using AutoMapper;
using Repositories.Entities;
using Services.DTOs;

namespace Services.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //Appointment
        CreateMap<Appointment, AppointmentDto>();
        CreateMap<CreateAppointmentDto, Appointment>();
        CreateMap<UpdateAppointmentDto, Appointment>()
            .ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember != null));
        
        //Account
        CreateMap<Account, AccountDto>();
        CreateMap<CreateAccountDto, Account>();
        CreateMap<Account, LoginResponse>();
        CreateMap<UpdateAccountDto, Account>()
            .ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember != null));
        
        //Checkpoint
        CreateMap<Checkpoint, CheckpointDto>();
        CreateMap<CreateCheckpointDto, Checkpoint>();
        CreateMap<UpdateCheckpointDto, Checkpoint>()
            .ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember != null));
        
        //CheckpointTask
        CreateMap<CheckpointTask, CheckpointTaskDto>();
        CreateMap<CreateCheckpointTaskDto, CheckpointTask>();
        CreateMap<UpdateCheckpointTaskDto, CheckpointTask>()
            .ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember != null));

        //Feedback
        CreateMap<Feedback, FeedbackDto>();
        CreateMap<CreateFeedbackDto, Feedback>();
        CreateMap<UpdateFeedbackDto, Feedback>()
            .ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember != null));


        //Group
        CreateMap<Group, GroupDto>();
        CreateMap<CreateGroupDto, Group>();
        CreateMap<UpdateGroupDto, Group>()
            .ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember != null));
            
        //Lecturer
        CreateMap<Lecturer, LecturerDto>();
        CreateMap<CreateLecturerDto, Lecturer>();
        CreateMap<UpdateLecturerDto, Lecturer>()
            .ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember != null));
            
        //Mentor
        CreateMap<Mentor, MentorDto>();
        CreateMap<CreateMentorDto, Mentor>();
        CreateMap<UpdateMentorDto, Mentor>()
            .ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember != null));
            
        //MentorAvailability
        CreateMap<MentorAvailability, MentorAvailabilityDto>();
        CreateMap<CreateMentorAvailabilityDto, MentorAvailability>();
        CreateMap<UpdateMentorAvailabilityDto, MentorAvailability>()
            .ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember != null));
            
        //Project
        CreateMap<Project, ProjectDto>();
        CreateMap<CreateProjectDto, Project>();
        CreateMap<UpdateProjectDto, Project>()
            .ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember != null));
            
        //Proposal
        CreateMap<Proposal, ProposalDto>();
        CreateMap<CreateProposalDto, Proposal>();   
        CreateMap<UpdateProposalDto, Proposal>()
            .ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember != null));
            
        //Student
        CreateMap<Student, StudentDto>();
        CreateMap<CreateStudentDto, Student>();
        CreateMap<UpdateStudentDto, Student>()
            .ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember != null));
            
        //StudentGroup
        CreateMap<StudentGroup, StudentGroupDto>();
        CreateMap<CreateStudentGroupDto, StudentGroup>();
        CreateMap<UpdateStudentGroupDto, StudentGroup>()
            .ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember != null));
            
        //TaskLog
        CreateMap<TaskLog, TaskLogDto>();
        CreateMap<CreateTaskLogDto, TaskLog>();
        CreateMap<UpdateTaskLogDto, TaskLog>()
            .ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember != null));
            
        //Transactions
        CreateMap<Transactions, TransactionsDto>();
        CreateMap<CreateTransactionsDto, Transactions>();
        CreateMap<UpdateTransactionsDto, Transactions>()
            .ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember != null)); 

        //WeeklyReports
        CreateMap<WeeklyReports, WeeklyReportsDto>();
        CreateMap<CreateWeeklyReportsDto, WeeklyReports>();
        CreateMap<UpdateWeeklyReportsDto, WeeklyReports>()
            .ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember != null));

    }
}