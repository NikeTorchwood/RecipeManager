using Application.Contracts.Abstractions;
using Application.Contracts.Ingredients;
using Core;

namespace Application.Contracts.Recipes;

public class RecipeFullDto : IFullDto<Recipe>
{
    public List<IngredientShortDto> Ingredients { get; set; }
}