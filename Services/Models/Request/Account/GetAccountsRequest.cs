using Repositories.Entities;
using Services.Models.Request.Base;

namespace Services.Models.Request.Account;

public class GetAccountsRequest : PaginationQuery
{
    public AccountRole[]? Roles { get; set; }
}