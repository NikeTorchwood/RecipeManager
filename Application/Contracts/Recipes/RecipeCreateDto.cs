using Application.Contracts.Abstractions;
using Core;

namespace Application.Contracts.Recipes;

public class RecipeCreateDto : ICreateDto<Recipe>
{
    public RecipeIngredientCreateDto[] Ingredients { get; init; }
    public string Description { get; init; }
    public string Name { get; init; }
    public Guid CategoryId { get; init; }
}