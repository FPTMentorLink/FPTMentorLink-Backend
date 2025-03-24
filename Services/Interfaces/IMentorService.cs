using Services.Models.Request.Mentor;
using Services.Models.Request.Student;
using Services.Models.Response.Appointment;
using Services.Models.Response.Mentor;
using Services.Models.Response.Transaction;
using Services.Utils;

namespace Services.Interfaces;

public interface IMentorService
{
    Task<Result<PaginationResult<TransactionResponse>>> GetMyTransactionPagedAsync(GetMentorTransactionsRequest request);
    Task<Result<PaginationResult<AppointmentResponse>>> GetMyAppointmentPagedAsync(GetMentorAppointmentsRequest request);

    Task<Result<PaginationResult<GetMentorResponse>>> GetMentorsPagedAsync(GetMentorsRequest request);
}