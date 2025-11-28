using Application.Contracts.RecipeIngredients;
using Application.Services.Abstractions.Base;
using Core;

namespace Application.Services.Abstractions;

public interface IRecipeIngredientService
    : IServiceBase<
        RecipeIngredient,
        RecipeIngredientShortDto,
        RecipeIngredientFullDto,
        RecipeIngredientFilter,
        RecipeIngredientCreateDto>;