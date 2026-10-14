using FluentValidation;
using Remp.Service.DTOs;

namespace Remp.Service.Validators;

public class UpdateListingCaseValidator : AbstractValidator<UpdateListingCaseDto>
{
    public UpdateListingCaseValidator()
    {
        RuleFor(l => l.Title).NotEmpty().WithMessage("Title cannot be empty!")
                            .MaximumLength(255).WithMessage("Title is too long!");
        RuleFor(l => l.Bathrooms).GreaterThanOrEqualTo(0).WithMessage("Bathrooms cannot be negative");
        RuleFor(l => l.Bedrooms).GreaterThanOrEqualTo(0).WithMessage("Bedrooms cannot be negative");
        RuleFor(l => l.Garages).GreaterThanOrEqualTo(0).WithMessage("Garages cannot be negative");
        RuleFor(l => l.Price).GreaterThan(0).WithMessage("Price must be greater than 0");
        RuleFor(l => l.FloorArea).GreaterThan(0).WithMessage("FloorArea must be greater than 0");
        RuleFor(l => l.SaleCategory).IsInEnum().WithMessage("Invalid sale category");
    }
}
