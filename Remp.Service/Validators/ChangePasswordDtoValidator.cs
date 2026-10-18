using System;
using FluentValidation;
using Remp.Service.DTOs;

namespace Remp.Service.Validators;

public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordDtoValidator()
    {
        RuleFor(u => u.CurrentPassword).NotEmpty().WithMessage("Current password cannot be empty!");
        RuleFor(u => u.NewPassword).NotEmpty().WithMessage("New password cannot be empty!");
        RuleFor(u => u.ConfirmPassword).NotEmpty().WithMessage("Confirm password cannot be empty!")
                                        .Equal(u=> u.NewPassword).WithMessage("Confirm Password and password are not same");

    }
}
