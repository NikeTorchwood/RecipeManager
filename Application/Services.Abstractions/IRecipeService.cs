using Application.Contracts.Recipes;
using Application.Services.Abstractions.Base;
using Core;

namespace Application.Services.Abstractions;

public interface IRecipeService
    : IServiceBase<
        Recipe,
        RecipeShortDto,
        RecipeFullDto,
        RecipeFilter,
        RecipeCreateDto>;