using System.ComponentModel.DataAnnotations;
using Services.Models.Request.Base;
using Services.Utils;

namespace Services.Models.Request.Student;

public class StudentDepositRequest : ValidatableObject
{
    [Range(Constants.MinDepositAmount, Constants.MaxDepositAmount)]
    public int Amount { get; set; }
}