using System;
using FluentValidation;
using Remp.Service.DTOs;

namespace Remp.Service.Validators;

public class UpdateListingCaseStatusDtoValidator : AbstractValidator<UpdateListingCaseStatusDto>
{
    public UpdateListingCaseStatusDtoValidator()
    {
        RuleFor(x => x.Status).IsInEnum().WithMessage("Invalid status");
    }
}
