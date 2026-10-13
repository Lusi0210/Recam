using FluentValidation;
using Remp.Service.DTOs;

namespace Remp.Service.Validators;

public class CreateListingCaseValidator : AbstractValidator<CreateListingCaseDto>
{
    public CreateListingCaseValidator()
    {
        RuleFor(l => l.Title).NotEmpty().WithMessage("Title cannot be empty!")
                            .MaximumLength(255).WithMessage("Title is too long!");
        RuleFor(l => l.Street).NotEmpty().WithMessage("Street cannot be empty");
        RuleFor(l => l.City).NotEmpty().WithMessage("City cannot be empty");
        RuleFor(l => l.State).NotEmpty().WithMessage("State cannot be empty");
        RuleFor(l => l.Postcode).NotEmpty().WithMessage("Postcode cannot be empty");
        RuleFor(l => l.Bathrooms).GreaterThanOrEqualTo(0).WithMessage("Bathrooms cannot be negative");
        RuleFor(l => l.Bedrooms).GreaterThanOrEqualTo(0).WithMessage("Bedrooms cannot be negative");
        RuleFor(l => l.Garages).GreaterThanOrEqualTo(0).WithMessage("Garages cannot be negative");
        RuleFor(l => l.Price).GreaterThan(0).WithMessage("Price must be greater than 0");
        RuleFor(l => l.FloorArea).GreaterThan(0).WithMessage("FloorArea must be greater than 0");
        RuleFor(l => l.Latitude).InclusiveBetween(-90m, 90m).WithMessage("Latitude must be between -90 and 90");
        RuleFor(l => l.Longitude).InclusiveBetween(-180m, 180m).WithMessage("Longitude must be between -180 and 180");
        RuleFor(l => l.PropertyType).IsInEnum().WithMessage("Invalid property type");
        RuleFor(l => l.SaleCategory).IsInEnum().WithMessage("Invalid sale category");
    }
}
