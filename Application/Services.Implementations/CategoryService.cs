using Application.Contracts.Categories;
using Application.Repositories.Abstractions;
using Application.Services.Abstractions;
using Application.Services.Implementations.Base;
using AutoMapper;
using Core;

namespace Application.Services.Implementations;

public class CategoryService(
    ICategoryRepository repository,
    IMapper mapper)
    : ServiceBase<
            Category,
            CategoryDto,
            CategoryDto,
            CategoryFilter,
            CategoryCreateDto>(repository, mapper),
        ICategoryService;