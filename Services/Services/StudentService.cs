using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;
using Services.Interfaces;
using Services.Models.Request.Student;
using Services.Models.Response.Student;
using Services.Models.VnPay;
using Services.Utils;

namespace Services.Services;

public class StudentService : IStudentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IVnPayService _vnPayService;

    public StudentService(IUnitOfWork unitOfWork, IVnPayService vnPayService)
    {
        _unitOfWork = unitOfWork;
        _vnPayService = vnPayService;
    }

    public async Task<Result<StudentDepositResponse>> DepositAsync(Guid id, StudentDepositRequest request,
        HttpContext httpContext)
    {
        var code = await _unitOfWork.Students.GetQueryable()
            .Where(x => x.Id == id)
            .Select(x => x.Code).FirstOrDefaultAsync();
        if (code == null)
        {
            return Result.Failure<StudentDepositResponse>("Student not found");
        }

        var transactionId = CodeGenerator.GenerateRandomNumber(8);
        var paymentModel = new CreateVnPayPaymentModel
        {
            TxnRef = transactionId,
            Amount = request.Amount,
            OrderType = "Other",
            OrderInfo = $"Nap tien cho MSSV {code} voi so tien {request.Amount} VND",
            IpAddr = Infrastructure.VnPay.Utils.GetIpAddress(httpContext)
        };
        var transaction = new Transaction
        {
            Code = CodeGenerator.GenerateTransactionCode(code, transactionId, request.Amount),
            VnPayTransactionId = transactionId,
            Description = paymentModel.OrderInfo,
            Type = TransactionType.Deposit,
            AccountId = id,
            Amount = request.Amount,
            Status = TransactionStatus.Pending
        };

        var paymentUrl = _vnPayService.CreatePaymentLink(paymentModel);
        if (paymentUrl == null)
        {
            return Result.Failure<StudentDepositResponse>("Failed to create payment link");
        }

        _unitOfWork.Transactions.Add(transaction);
        await _unitOfWork.SaveChangesAsync();
        return new StudentDepositResponse
        {
            PaymentUrl = paymentUrl
        };
    }

    public async Task<VnPayIpnResponse> HandleVnPayIpn(IQueryCollection request)
    {
        var verifyResult = _vnPayService.VerifyIpnPayment(request);
        if (verifyResult.RspCode != "00") return verifyResult;
        var txnRef = request["vnp_TxnRef"];
        var vnPayTransactionId = long.Parse(txnRef!);
        var transaction =
            await _unitOfWork.Transactions.FindSingleAsync(x => x.VnPayTransactionId == vnPayTransactionId);
        if (transaction == null)
            return new VnPayIpnResponse
            {
                RspCode = "01",
                Message = "Transaction not found"
            };
        var amount = int.Parse(request["vnp_Amount"]!) / 100;
        if (transaction.Amount != amount)
        {
            transaction.Status = TransactionStatus.Failed;
            return new VnPayIpnResponse
            {
                RspCode = "04",
                Message = "Invalid Amount"
            };
        }

        if (transaction.Status == TransactionStatus.Success)
        {
            return new VnPayIpnResponse
            {
                RspCode = "02",
                Message = "Transaction already confirmed"
            };
        }

        if (transaction.Status == TransactionStatus.Failed)
        {
            return new VnPayIpnResponse
            {
                RspCode = "99",
                Message = "Transaction already failed"
            };
        }

        var responseCode = request["vnp_ResponseCode"];
        var transactionStatus = request["vnp_TransactionStatus"];
        if (responseCode == "00" && transactionStatus == "00")
        {
            transaction.Status = TransactionStatus.Success;
            var student = await _unitOfWork.Students.FindSingleAsync(x => x.Id == transaction.AccountId);
            if (student == null) return new VnPayIpnResponse
            {
                RspCode = "99",
                Message = "Transaction failed"
            };
            student.Balance += transaction.Amount;
            _unitOfWork.Students.Update(student);
            _unitOfWork.Transactions.Update(transaction);
            await _unitOfWork.SaveChangesAsync();
            return new VnPayIpnResponse
            {
                RspCode = "00",
                Message = "Transaction confirmed"
            };
        }

        transaction.Status = TransactionStatus.Failed;
        _unitOfWork.Transactions.Update(transaction);
        await _unitOfWork.SaveChangesAsync();
        return new VnPayIpnResponse
        {
            RspCode = "99",
            Message = "Transaction failed"
        };
    }
}