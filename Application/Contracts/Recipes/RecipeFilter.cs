using Application.Contracts.Base;
using Core;

namespace Application.Contracts.Recipes;

public class RecipeFilter : FilterBase<Recipe>
{
    public string? Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public Guid? CategoryId { get; set; } = null;
    public Guid? IngredientId { get; set; } = null;
    public DateTimeOffset? CreatedFrom { get; set; } = null;
    public DateTimeOffset? CreatedTo { get; set; } = null;
}