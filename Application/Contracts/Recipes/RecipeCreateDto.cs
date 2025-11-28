using Application.Contracts.Abstractions;
using Application.Contracts.Ingredients;
using Core;

namespace Application.Contracts.Recipes;

public class RecipeCreateDto : ICreateDto<Recipe>
{
    public IngredientCreateDto[] Ingredients { get; set; }
}