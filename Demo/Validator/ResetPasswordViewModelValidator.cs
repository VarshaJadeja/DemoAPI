using Demo.Entities.ViewModels;
using FluentValidation;

public class ResetPasswordViewModelValidator : AbstractValidator<ResetPasswordViewModel>
{
    public ResetPasswordViewModelValidator()
    {
        RuleFor(x => x.NewPassword).NotEmpty().WithMessage("New Password is required.");
        RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("Confirm Password is required.")
            .Equal(x => x.NewPassword)
            .WithMessage("Passwords do not match.");
    }
}
