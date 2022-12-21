using System.ComponentModel.DataAnnotations;

namespace NewsAggregator.WebAPI.Models.Requests;

public class LoginUserRequestModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}