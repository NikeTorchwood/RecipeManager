using Application.Contracts.Recipes;
using Application.Exceptions;
using Application.Repositories.Abstractions;
using Application.Services.Abstractions;
using Application.Services.Implementations.Base;
using AutoMapper;
using Core;

namespace Application.Services.Implementations;

public class RecipeService(
    IRecipeRepository recipeRepository,
    IIngredientRepository ingredientRepository,
    IRecipeIngredientRepository recipeIngredientRepository,
    IMapper mapper)
    : ServiceBase<
            Recipe,
            RecipeShortDto,
            RecipeFullDto,
            RecipeFilter,
            RecipeCreateDto>(recipeRepository, mapper),
        IRecipeService
{
    public override async Task<Guid> CreateAsync(RecipeCreateDto request, CancellationToken token = default)
    {
        if (request.Ingredients != null && request.Ingredients.Length != 0)
        {
            var requestedIngredientIds = request.Ingredients
                .Select(x => x.IngredientId)
                .Distinct()
                .ToArray();

            var existingIds = await ingredientRepository.ExistingIdsAsync(requestedIngredientIds, token);

            var missingIds = requestedIngredientIds.Except(existingIds).ToArray();

            if (missingIds.Any())
            {
                throw new BadRequestException($"Ingredients with ids not found: {string.Join(", ", missingIds)}");
            }
        }

        using var scope = new System.Transactions.TransactionScope(
            System.Transactions.TransactionScopeAsyncFlowOption.Enabled);

        var recipeId = await base.CreateAsync(request, token);

        if (request.Ingredients != null && request.Ingredients.Length != 0)
        {
            var links = request.Ingredients.Select(i => new RecipeIngredient
            {
                RecipeId = recipeId,
                IngredientId = i.IngredientId,
                Quantity = i.Quantity
            });

            await recipeIngredientRepository.AddRangeAsync(links, token);
        }

        scope.Complete();

        return recipeId;
    }

    public override async Task<RecipeFullDto?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        var fullDto = await recipeRepository.GetFullByIdAsync(id, token);

        return fullDto;
    }

    public override async Task DeleteAsync(Guid id, CancellationToken token = default)
    {
        using var scope = new System.Transactions.TransactionScope(
            System.Transactions.TransactionScopeAsyncFlowOption.Enabled);

        await recipeIngredientRepository.DeleteByRecipeIdAsync(id, token);

        await base.DeleteAsync(id, token);

        scope.Complete();
    }
}