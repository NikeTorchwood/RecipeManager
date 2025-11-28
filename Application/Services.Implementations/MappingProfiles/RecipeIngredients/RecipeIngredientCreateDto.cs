using Application.Contracts.Abstractions;
using Core;

namespace Application.Services.Implementations.MappingProfiles.RecipeIngredients;

public class RecipeIngredientCreateDto : ICreateDto<RecipeIngredient>
{
    public Guid RecipeId { get; set; }
    public Guid IngredientId { get; set; }
}