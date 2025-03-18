using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Repositories.Entities;
using Services.Models.Request.Base;
using Services.Utils;

namespace Services.Models.Request.Student;

public class GetStudentProjectsRequest : PaginationQuery
{
    [BindNever] public Guid? StudentId { get; set; }
    public Guid? TermId { get; set; }
    public ProjectStatus? Status { get; set; }
    [BindNever] public new string? SearchTerm { get; set; }
    [BindNever] public new string? OrderBy { get; set; }
    [BindNever] public new bool IsDescending { get; set; }
}