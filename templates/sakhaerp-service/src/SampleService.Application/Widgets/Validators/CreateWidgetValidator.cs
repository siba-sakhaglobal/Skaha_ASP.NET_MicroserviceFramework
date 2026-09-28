namespace SampleService.Application.Widgets.Validators;

using FluentValidation;
using SampleService.Application.Widgets.Commands;

public class CreateWidgetValidator : AbstractValidator<CreateWidgetCommand>
{
    public CreateWidgetValidator()
    {
        RuleFor(x => x.Request.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Request.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Request.Quantity).GreaterThanOrEqualTo(0);
    }
}
