using FluentValidation;
using TaskTracker.API.Application.Common.Exceptions;

namespace TaskTracker.API.Application.Common.Helpers;

public static class ValidationHelper
{
    public static async Task ValidateAndThrowAsync<T>(IValidator<T> validator, T instance, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(instance, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            throw new BadRequestException("Validation failed.", errors);
        }
    }
}
