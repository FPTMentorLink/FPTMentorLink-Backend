using Microsoft.EntityFrameworkCore;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;
using Services.Interfaces;
using Services.Models.Request.Appointment;
using Services.Models.Request.Mentor;
using Services.Models.Response.Appointment;
using Services.Models.Response.Transaction;
using Services.Utils;

namespace Services.Services;

public class MentorService : IMentorService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppointmentService _appointmentService;

    public MentorService(IUnitOfWork unitOfWork, IAppointmentService appointmentService)
    {
        _unitOfWork = unitOfWork;
        _appointmentService = appointmentService;
    }

    public async Task<Result<PaginationResult<AppointmentResponse>>> GetMyAppointmentPagedAsync(
        GetMentorAppointmentsRequest request)
    {
        return await _appointmentService.GetPagedAsync(new GetAppointmentsRequest
        {
            StudentId = request.MentorId,
            Status = request.Status,
            SearchTerm = request.SearchTerm,
            OrderBy = request.OrderBy,
            IsDescending = request.IsDescending,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        });
    }

    public async Task<Result<PaginationResult<TransactionResponse>>> GetMyTransactionPagedAsync(
        GetMentorTransactionsRequest request)
    {
        var query = _unitOfWork.Transactions.FindAll()
            .Include(x => x.Account)
            .Where(x => x.AccountId == request.MentorId);

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(x => 
                x.Code.Contains(request.SearchTerm) || 
                x.Description.Contains(request.SearchTerm));
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(x => x.CreatedAt >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(x => x.CreatedAt <= request.ToDate.Value);
        }

        query = query.OrderByDescending(x => x.CreatedAt);

        var result = await query.ProjectToPaginatedListAsync<Transaction, TransactionResponse>(request);
        return Result.Success(result);
    }
}
