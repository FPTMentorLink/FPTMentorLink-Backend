using Microsoft.EntityFrameworkCore;
using Repositories.Entities;

namespace Services.Models.Request.CheckpointTask;

public class UpdateCheckpointTaskStatusRequest : DbLoggerCategory.Database.Command
{
    public CheckpointTaskStatus? Status { get; set; }
}