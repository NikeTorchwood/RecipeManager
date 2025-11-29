using Application.Contracts.Abstractions;
using Core;

namespace Application.Contracts.Recipes;

public class RecipeIngredientDto : IFullDto<RecipeIngredient>, IShortDto<RecipeIngredient>
{
    public Guid Id { get; init; }
    public Guid IngredientId { get; init; }
    public string Name { get; init; }
    public string Quantity { get; init; }
}