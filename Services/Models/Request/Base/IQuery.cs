using Services.Utils;

namespace Services.Models.Request.Base;

public interface IQuery
{
}

public abstract class PaginationQuery : PaginationParams, IQuery;