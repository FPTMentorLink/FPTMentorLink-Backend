﻿using Repositories.Entities;
using Services.Models.Response.Base;

namespace Services.Models.Response.CheckpointTask;

public class CheckpointTaskResponse : AuditableResponse
{
    public Guid Id { get; set; }
    public Guid CheckpointId { get; set; }
    public string CheckpointName { get; set; } = null!;
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public CheckpointTaskStatus Status { get; set; }
    public string StatusName => Status.ToString();
    public double? Score { get; set; }
}