﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Repositories.Entities.Base;

namespace Repositories.Entities;

public class WeeklyReportFeedback : AuditableEntity
{
    [ForeignKey(nameof(WeeklyReport))] public Guid WeeklyReportId { get; set; }
    [ForeignKey(nameof(Lecturer))] public Guid LecturerId { get; set; }
    [MaxLength(2000)] public string Content { get; set; } = null!;

    public virtual WeeklyReport WeeklyReport { get; set; } = null!;
    public virtual Lecturer Lecturer { get; set; } = null!;
}