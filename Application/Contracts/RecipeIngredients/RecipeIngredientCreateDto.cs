using Application.Contracts.Abstractions;
using Core;

namespace Application.Contracts.RecipeIngredients;

public class RecipeIngredientCreateDto : ICreateDto<RecipeIngredient>
{
    public Guid RecipeId { get; set; }
    public Guid IngredientId { get; set; }
}