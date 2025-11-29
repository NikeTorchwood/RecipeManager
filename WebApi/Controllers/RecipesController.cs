using Application.Contracts.Recipes;
using Application.Services.Abstractions;
using Core;

namespace WebApi.Controllers;

public class RecipesController(IRecipeService service)
    : ApiControllerBase<
            Recipe,
            RecipeShortDto,
            RecipeFullDto,
            RecipeFilter,
            RecipeCreateDto>
        (service);