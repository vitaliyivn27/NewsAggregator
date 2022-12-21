using System.ComponentModel.DataAnnotations;

namespace NewsAggregator.WebAPI.Models.Requests;

public class RegisterUserRequestModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Compare(nameof(Password))]
    [DataType(DataType.Password)]
    public string PasswordConfirmation { get; set; }
}