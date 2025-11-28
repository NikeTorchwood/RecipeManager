using Application.Contracts.Ingredients;
using Application.Services.Abstractions.Base;
using Core;

namespace Application.Services.Abstractions;

public interface IIngredientService
    : IServiceBase<
        Ingredient,
        IngredientShortDto,
        IngredientFullDto,
        IngredientFilter,
        IngredientCreateDto>;