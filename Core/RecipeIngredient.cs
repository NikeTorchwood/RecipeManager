using Core.Abstractions;
using Dapper.Contrib.Extensions;

namespace Core;

[Table(Constats.RecipeIngredients)]
public class RecipeIngredient : IEntity
{
    [Key]
    public Guid Id { get; init; }

    public Guid RecipeId { get; init; }
    public Guid IngredientId { get; init; }
    public string Quantity { get; init; }
}