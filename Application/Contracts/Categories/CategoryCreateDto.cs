using Application.Contracts.Abstractions;
using Core;

namespace Application.Contracts.Categories;

public class CategoryCreateDto : ICreateDto<Category>
{
    public string Name { get; set; }
}