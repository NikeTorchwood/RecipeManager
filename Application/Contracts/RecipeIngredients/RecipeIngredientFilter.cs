using Application.Contracts.Base;
using Core;

namespace Application.Contracts.RecipeIngredients;

public class RecipeIngredientFilter : FilterBase<RecipeIngredient>
{
    public Guid? RecipeId { get; set; } = null;
    public Guid? IngredientId { get; set; } = null;
    public string? Quantity { get; set; } = string.Empty;
}