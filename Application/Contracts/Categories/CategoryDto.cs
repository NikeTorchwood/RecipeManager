using Application.Contracts.Abstractions;
using Core;

namespace Application.Contracts.Categories;

public class CategoryDto : IFullDto<Category>, IShortDto<Category>
{
    public Guid Id { get; init; }
    public string Name { get; init; }
}