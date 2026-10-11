using System;
using System.Data;
using FluentValidation;
using Remp.Service.DTOs;

namespace Remp.Service.Validators;

public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto> 
{
    public LoginRequestDtoValidator()
    {
        RuleFor(u=> u.Email).NotEmpty().WithMessage("Email can not be empty!")
                            .EmailAddress().WithMessage("Must be Email address!");
        RuleFor(u => u.Password).NotEmpty().WithMessage("Password can not be empty!");
    }
}
