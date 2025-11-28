using Application.Contracts.Base;
using Core;

namespace Application.Contracts.Categories;

public class CategoryFilter : FilterBase<Category>
{
    public string? Name { get; set; } = string.Empty;
}