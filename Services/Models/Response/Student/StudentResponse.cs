using Services.Models.Response.Account;
using Services.Models.Response.Base;

namespace Services.Models.Response.Student;

public class StudentResponse : BaseAccountResponse
{
    public required string Code { get; set; }
    public int Balance { get; set; }
    public string Faculty { get; set; }
}