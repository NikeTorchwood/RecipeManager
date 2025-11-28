using Core.Abstractions;
using Dapper.Contrib.Extensions;

namespace Core;

[Table(Constats.Ingredients)]
public class Ingredient : IEntity
{
    [Key]
    public Guid Id { get; }
    public string Name { get; }
}