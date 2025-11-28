using Core.Abstractions;
using Dapper.Contrib.Extensions;

namespace Core;

[Table(Constats.Categories)]
public class Category : IEntity
{
    [Key]
    public Guid Id { get; }
    public string Name { get; }
}