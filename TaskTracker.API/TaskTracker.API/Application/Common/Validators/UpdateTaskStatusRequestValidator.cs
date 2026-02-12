using FluentValidation;
using TaskTracker.API.Application.DTOs;

namespace TaskTracker.API.Application.Common.Validators;

public class UpdateTaskStatusRequestValidator : AbstractValidator<UpdateTaskStatusRequest>
{
    public UpdateTaskStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid status value.");
    }
}
