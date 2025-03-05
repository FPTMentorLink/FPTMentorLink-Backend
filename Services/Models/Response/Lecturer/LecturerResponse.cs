using Services.Models.Response.Account;
using Services.Models.Response.Base;

namespace Services.Models.Response.Lecturer;

public class LecturerResponse  : BaseAccountResponse
{
    public string Code { get; set; } = null!;
    public string? Description { get; set; }
    public string? Faculty { get; set; }
}