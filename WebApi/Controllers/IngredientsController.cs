using Application.Contracts.Ingredients;
using Application.Services.Abstractions;
using Core;

namespace WebApi.Controllers;

public class IngredientsController(IIngredientService service)
    : ApiControllerBase<
            Ingredient,
            IngredientDto,
            IngredientDto,
            IngredientFilter,
            IngredientCreateDto>
        (service);