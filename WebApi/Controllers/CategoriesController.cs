using Application.Contracts.Categories;
using Application.Services.Abstractions;
using Core;

namespace WebApi.Controllers;

public class CategoriesController(ICategoryService service)
    : ApiControllerBase<
            Category,
            CategoryShortDto,
            CategoryFullDto,
            CategoryFilter,
            CategoryCreateDto>
        (service);