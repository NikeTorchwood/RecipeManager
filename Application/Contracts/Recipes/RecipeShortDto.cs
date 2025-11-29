using Application.Contracts.Abstractions;
using Core;

namespace Application.Contracts.Recipes;

public class RecipeShortDto : IShortDto<Recipe>
{
    public Guid Id { get; init; }
    public string Name { get;  init;}
    public string Description { get; init; }
    public Guid CategoryId { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}