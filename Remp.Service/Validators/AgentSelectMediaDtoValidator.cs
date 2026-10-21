using System;
using FluentValidation;
using Remp.Service.DTOs;

namespace Remp.Service.Validators;

public class AgentSelectMediaDtoValidator : AbstractValidator<AgentSelectMediaDto>
{
    public AgentSelectMediaDtoValidator()
    {
        RuleFor(d => d.MediaAssetsId)
            .NotNull().WithMessage("Media ids cannot be null.")
            .Must(ids => ids.Count <= 10).WithMessage("You can select at most 10 images.")
            .Must(ids => ids.Distinct().Count() == ids.Count).WithMessage("Duplicate media ids are not allowed.");

        RuleForEach(d => d.MediaAssetsId)
            .GreaterThan(0).WithMessage("Media id must be greater than 0.");
    }
}
