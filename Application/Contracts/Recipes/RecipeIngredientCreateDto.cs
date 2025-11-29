using Application.Contracts.Abstractions;
using Core;

namespace Application.Contracts.Recipes;

public class RecipeIngredientCreateDto : ICreateDto<RecipeIngredient>
{
    public Guid IngredientId { get; set; }
    public string Quantity { get; set; }
}