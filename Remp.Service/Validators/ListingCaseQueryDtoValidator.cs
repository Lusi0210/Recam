using System;
using FluentValidation;
using Remp.Service.DTOs;

namespace Remp.Service.Validators;

public class ListingCaseQueryDtoValidator : AbstractValidator<ListingCaseQueryDto>
{   
    private static readonly string[] AllowedSortFields = { "title", "city", "price", "status" };
    private static readonly string[] AllowedSortDirections = { "asc", "desc" };
    public ListingCaseQueryDtoValidator()
    {
        Include(new PaginationQueryDtoValidator());

        RuleFor(q => q.Status).IsInEnum().When(q => q.Status.HasValue)
            .WithMessage("Invalid status.");
        RuleFor(q => q.PropertyType).IsInEnum().When(q => q.PropertyType.HasValue)
            .WithMessage("Invalid property type.");
        RuleFor(q => q.SaleCategory).IsInEnum().When(q => q.SaleCategory.HasValue)
            .WithMessage("Invalid sale category.");

        RuleFor(q => q.MinPrice).GreaterThanOrEqualTo(0).When(q => q.MinPrice.HasValue)
            .WithMessage("Min price cannot be negative.");
        RuleFor(q => q.MaxPrice).GreaterThanOrEqualTo(0).When(q => q.MaxPrice.HasValue)
            .WithMessage("Max price cannot be negative.");
        RuleFor(q => q)
            .Must(q => q.MinPrice <= q.MaxPrice)
            .When(q => q.MinPrice.HasValue && q.MaxPrice.HasValue)
            .WithMessage("Min price cannot be greater than max price.");
        RuleFor(q => q.SortBy)
        .Must(s => s == null || AllowedSortFields.Contains(s.ToLower()))
        .WithMessage("SortBy must be one of: title, city, price, status");

        RuleFor(q => q.SortDirection)
            .Must(d => d == null || AllowedSortDirections.Contains(d.ToLower()))
            .WithMessage("SortDirection must be asc or desc");
    }
}
