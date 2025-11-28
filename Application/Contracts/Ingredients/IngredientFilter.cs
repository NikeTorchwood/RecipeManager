using Application.Contracts.Base;
using Core;

namespace Application.Contracts.Ingredients;

public class IngredientFilter : FilterBase<Ingredient>
{
    public string? Name { get; set; } = string.Empty;
}