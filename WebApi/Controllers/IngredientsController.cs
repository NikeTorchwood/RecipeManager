using Application.Contracts.Ingredients;
using Application.Contracts.RecipeIngredients;
using Application.Contracts.Recipes;
using Application.Services.Abstractions;
using Core;

namespace WebApi.Controllers;

public class IngredientsController(IIngredientService service)
    : ApiControllerBase<
            Ingredient,
            IngredientShortDto,
            IngredientFullDto,
            IngredientFilter,
            IngredientCreateDto>
        (service);

public class RecipesController(IRecipeService service)
    : ApiControllerBase<
            Recipe,
            RecipeShortDto,
            RecipeFullDto,
            RecipeFilter,
            RecipeCreateDto>
        (service);
public class RecipeIngredientsController(IRecipeIngredientService service)
    : ApiControllerBase<
            RecipeIngredient,
            RecipeIngredientShortDto,
            RecipeIngredientFullDto,
            RecipeIngredientFilter,
            RecipeIngredientCreateDto>
        (service);