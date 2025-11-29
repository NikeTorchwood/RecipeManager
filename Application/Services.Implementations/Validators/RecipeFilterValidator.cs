using Application.Contracts.Recipes;
using FluentValidation;

namespace Application.Validators;

public class RecipeFilterValidator : AbstractValidator<RecipeFilter>
{
    public RecipeFilterValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("Номер страницы должен быть >= 1");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(100).WithMessage("Размер страницы не может превышать 100 записей");

        RuleFor(x => x)
            .Must(x =>
                !x.CreatedFrom.HasValue ||
                !x.CreatedTo.HasValue ||
                x.CreatedFrom <= x.CreatedTo)
            .WithMessage("Дата 'C' (CreatedFrom) должна быть меньше или равна дате 'По' (CreatedTo)");

        RuleFor(x => x.Name)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.SortColumn)
            .Must(name =>
                string.IsNullOrEmpty(name) ||
                new[] { "name", "createdat", "description" }.Contains(name.ToLower()))
            .WithMessage("Сортировка возможна только по имени, дате создания, описанию");
    }
}