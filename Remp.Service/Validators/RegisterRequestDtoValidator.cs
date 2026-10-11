using System;
using FluentValidation;
using Remp.Service.DTOs;

namespace Remp.Service.Validators;

public class RegisterRequestDtoValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestDtoValidator()
    {
        RuleFor(u => u.Email).NotEmpty().WithMessage("Email can not be empty!")
                            .EmailAddress().WithMessage("Must be Email address")
                            .MaximumLength(256).WithMessage("Email is too long!");
        
        RuleFor(u => u.Password).NotEmpty().WithMessage("Password cannot be empty.")
                                .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
                                .Matches("[A-Z]").WithMessage("Password must contain an uppercase letter.")
                                .Matches("[a-z]").WithMessage("Password must contain a lowercase letter.")
                                .Matches("[0-9]").WithMessage("Password must contain a digit.")
                                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain a special character.");
        RuleFor(u => u.ConfirmPassword).Equal(u=> u.Password).WithMessage("Confirm Password and password are not same");                           
    }
}
