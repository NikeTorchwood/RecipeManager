using Application.Contracts.Recipes;
using FluentValidation;

namespace Application.Validators;

public class RecipeIngredientCreateDtoValidator : AbstractValidator<RecipeIngredientCreateDto>
{
    public RecipeIngredientCreateDtoValidator()
    {
        RuleFor(x => x.IngredientId)
            .NotEmpty().WithMessage("ID ингредиента обязателен")
            .NotEqual(Guid.Empty).WithMessage("Некорректный ID ингредиента");

        RuleFor(x => x.Quantity)
            .NotEmpty().WithMessage("Укажите количество (например, '200 гр')")
            .MaximumLength(100);
    }
}