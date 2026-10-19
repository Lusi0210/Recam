using System;
using FluentValidation;
using Microsoft.Identity.Client;
using Remp.Service.DTOs;

namespace Remp.Service.Validators;

public class AddCaseContactValidator : AbstractValidator<AddCaseContactDto>
{
    public AddCaseContactValidator()
    {
        RuleFor(c => c.FirstName).NotEmpty().WithMessage("First name can not be empty!");
        RuleFor(c => c.LastName).NotEmpty().WithMessage("Last name can not be empty!");
        RuleFor(c => c.Email).NotEmpty().WithMessage("Email can not be empty!")
                            .EmailAddress().WithMessage("Must be Email address");
        RuleFor(c => c.PhoneNumber).NotEmpty().WithMessage("Phone number can not be empty");
    }
}
