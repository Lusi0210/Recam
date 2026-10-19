using System;
using FluentValidation;
using Remp.Service.DTOs;

namespace Remp.Service.Validators;

public class AddAgentToListingCaseDtoValidator : AbstractValidator<AddAgentToListingCaseDto>
{
    public AddAgentToListingCaseDtoValidator()
    {
        RuleFor(a => a.Id).NotEmpty().WithMessage("Agent id can not be empty!");
    }
}
