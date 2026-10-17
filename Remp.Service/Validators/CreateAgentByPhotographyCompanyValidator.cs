using System;
using FluentValidation;
using Remp.Service.DTOs;

namespace Remp.Service.Validators;

public class CreateAgentByPhotographyCompanyValidator : AbstractValidator<CreateAgentByPhotographyCompanyDto>
{
    public CreateAgentByPhotographyCompanyValidator()
    {
        RuleFor(a => a.AgentFirstName).NotEmpty().WithMessage("First name cannot be empty!");
        RuleFor(a => a.AgentLastName).NotEmpty().WithMessage("Last name cannot be empty!");
        RuleFor(a => a.Email).NotEmpty().WithMessage("Email cannot be empty!")
                            .EmailAddress().WithMessage("Must be email address!");
    }
}
