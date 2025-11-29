using Application.Contracts.Abstractions;
using Application.Contracts.Ingredients;
using Core;

namespace Application.Contracts.Recipes;

public class RecipeFullDto : IFullDto<Recipe>
{
    public Guid Id { get; init; }
    public string Name { get;  init;}
    public string Description { get; init; }
    public Guid CategoryId { get;  init;}
    public DateTimeOffset CreatedAt { get; init; }
    public List<RecipeIngredientDto> Ingredients { get; set; }
}