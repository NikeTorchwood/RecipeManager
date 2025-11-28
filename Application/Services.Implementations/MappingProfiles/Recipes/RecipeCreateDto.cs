using Application.Contracts.Abstractions;
using Core;

namespace Application.Services.Implementations.MappingProfiles.Recipes;

public class RecipeCreateDto : ICreateDto<Recipe>
{
    public IngredientCreateDto[] Ingredients { get; set; }
}