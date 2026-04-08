namespace InventoryService.Application.ItemWarehouseMappings.Validators;

using FluentValidation;
using InventoryService.Application.ItemWarehouseMappings.Commands;

public class CreateItemWarehouseMappingValidator : AbstractValidator<CreateItemWarehouseMappingCommand>
{
    public CreateItemWarehouseMappingValidator()
    {
        RuleFor(x => x.Request.ItemId).NotEmpty();
        RuleFor(x => x.Request.ItemCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Request.WarehouseId).NotEmpty();
        RuleFor(x => x.Request.WarehouseCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Request.OpeningQuantity).GreaterThanOrEqualTo(0).When(x => x.Request.OpeningQuantity.HasValue);
        RuleFor(x => x.Request.OpeningRate).GreaterThanOrEqualTo(0).When(x => x.Request.OpeningRate.HasValue);
        RuleFor(x => x.Request.OpeningRate).NotNull().When(x => x.Request.OpeningQuantity > 0)
            .WithMessage("Opening rate is required when opening quantity > 0");
        RuleFor(x => x.Request.MinQuantity).GreaterThanOrEqualTo(0).When(x => x.Request.MinQuantity.HasValue);
        RuleFor(x => x.Request.MaxQuantity).GreaterThan(0).When(x => x.Request.MaxQuantity.HasValue);
        RuleFor(x => x.Request.MaxQuantity).GreaterThanOrEqualTo(x => x.Request.MinQuantity ?? 0)
            .When(x => x.Request.MaxQuantity.HasValue && x.Request.MinQuantity.HasValue)
            .WithMessage("Max quantity must be >= min quantity");
    }
}
