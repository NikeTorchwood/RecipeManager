using Application.Contracts.Categories;
using Application.Services.Abstractions.Base;
using Core;

namespace Application.Services.Abstractions;

public interface ICategoryService
    : IServiceBase<
        Category,
        CategoryShortDto,
        CategoryFullDto,
        CategoryFilter,
        CategoryCreateDto>;