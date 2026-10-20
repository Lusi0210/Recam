using System;
using FluentValidation;
using Remp.Service.DTOs;

namespace Remp.Service.Validators;

public class SetCoverImageDtoValidator : AbstractValidator<SetCoverImageDto>
{
    public SetCoverImageDtoValidator()
    {
        RuleFor(c => c.MediaId).GreaterThan(0).WithMessage("Media id must be greater than 0.");
    }
}
