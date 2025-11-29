using Core.Abstractions;
using Dapper.Contrib.Extensions;

namespace Core;

[Table(Constats.Recipes)]
public class Recipe : IEntity
{
    [Key] public Guid Id { get; }
    public string Name { get; init; }
    public string Description { get; init; }
    public Guid CategoryId { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}