using System.ComponentModel.DataAnnotations;

namespace Demo.Entities.ViewModels;

public class ResetPasswordViewModel 
{
    public string? Token { get; set; }
    public string? Email { get; set; }
    public string? Expiration { get; set; }
    public bool IsLinkValid { get; set; } = true;
    public bool IsPasswordUpdated { get; set; }
    public bool IsSubmitted { get; set; }
    public string? Password { get; set; }

    [Required(ErrorMessage = "NewPassword is Required")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "ConfirmPassword is Required")]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Password and ConfirmPassword must be equal.")]
    public string ConfirmPassword { get; set; }

}

