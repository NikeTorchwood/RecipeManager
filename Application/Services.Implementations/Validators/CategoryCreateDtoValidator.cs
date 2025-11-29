using Application.Contracts.Categories;
using FluentValidation;

namespace Application.Validators;

public class CategoryCreateDtoValidator : AbstractValidator<CategoryCreateDto>
{
    public CategoryCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Название категории не может быть пустым")
            .MaximumLength(255).WithMessage("Название слишком длинное (макс 255)");
    }
}