using FluentValidation;
using TaskTracker.API.Application.DTOs;

namespace TaskTracker.API.Application.Common.Validators;

public class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(50).WithMessage("Title cannot exceed 50 characters.");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Invalid priority value.");
    }
}
