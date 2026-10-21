using System;
using FluentValidation;
using FluentValidation.Results;
using Remp.Common.Exceptions;

namespace Remp.Service.Common;

public static class ValidatorExtensions
{
    public static async Task ValidateOrThrowAsync<T>(this IValidator<T> validator, T instance)
    {
        ValidationResult result = await validator.ValidateAsync(instance);
        if (!result.IsValid)
        {
            List<string> errors = result.Errors.Select(e => e.ErrorMessage).ToList();
            throw new BadRequestException("Validation failed", errors);
        }
    }
}
