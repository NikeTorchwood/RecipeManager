using Application.Contracts.Recipes;
using FluentValidation;

namespace Application.Validators;

public class RecipeCreateDtoValidator : AbstractValidator<RecipeCreateDto>
{
    public RecipeCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Название рецепта обязательно")
            .MaximumLength(255);

        RuleFor(x => x.Description)
            .MaximumLength(4000);

        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .NotEqual(Guid.Empty).WithMessage("Выберите категорию");

        RuleForEach(x => x.Ingredients)
            .SetValidator(new RecipeIngredientCreateDtoValidator());
    }
}