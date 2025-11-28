using Core.Abstractions;
using Dapper.Contrib.Extensions;

namespace Core;

[Table(Constats.Recipes)]
public class Recipe : IEntity
{
    [Key]
    public Guid Id { get; }
    public string Name { get; }
    public string Description { get; }
    public Guid CategoryId { get; }
    public DateTimeOffset CreatedAt { get; }
}