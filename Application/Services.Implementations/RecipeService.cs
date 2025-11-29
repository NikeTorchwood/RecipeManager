using System.Transactions;
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
        await ValidateIngredientsAsync(request, token);

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var recipeId = await base.CreateAsync(request, token);

        await AddIngredientsToRecipeAsync(recipeId, request.Ingredients, token);

        scope.Complete();

        return recipeId;
    }

    public override async Task UpdateAsync(Guid id, RecipeCreateDto request, CancellationToken token = default)
    {
        await ValidateIngredientsAsync(request, token);

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await base.UpdateAsync(id, request, token);

        var existingLinks = (await recipeIngredientRepository.GetByRecipeIdAsync(id, token)).ToList();

        var incomingIngredients = request.Ingredients ?? [];

        var incomingMap = incomingIngredients
            .DistinctBy(x => x.IngredientId)
            .ToDictionary(x => x.IngredientId, x => x);

        var toDeleteIds = existingLinks
            .Where(dbLink => !incomingMap.ContainsKey(dbLink.IngredientId))
            .Select(dbLink => dbLink.Id)
            .ToArray();

        var toAdd = incomingMap.Values
            .Where(dto => existingLinks.All(dbLink => dbLink.IngredientId != dto.IngredientId))
            .Select(dto => new RecipeIngredient
            {
                RecipeId = id,
                IngredientId = dto.IngredientId,
                Quantity = dto.Quantity
            })
            .ToList();

        var toUpdate = new List<RecipeIngredient>();
        foreach (var dbLink in existingLinks)
        {
            if (incomingMap.TryGetValue(dbLink.IngredientId, out var dto))
            {
                if (dbLink.Quantity != dto.Quantity)
                {
                    toUpdate.Add(new RecipeIngredient
                    {
                        Id = dbLink.Id,
                        RecipeId = id,
                        IngredientId = dbLink.IngredientId,
                        Quantity = dto.Quantity
                    });
                }
            }
        }

        if (toDeleteIds.Any())
        {
            await recipeIngredientRepository.DeleteManyAsync(toDeleteIds, token);
        }

        if (toAdd.Any())
        {
            await recipeIngredientRepository.AddRangeAsync(toAdd, token);
        }

        foreach (var item in toUpdate)
        {
            await recipeIngredientRepository.UpdateAsync(item, token);
        }

        scope.Complete();
    }

    public override async Task<RecipeFullDto?> GetByIdAsync(Guid id, CancellationToken token = default)
        => await recipeRepository.GetFullByIdAsync(id, token);

    public override async Task DeleteAsync(Guid id, CancellationToken token = default)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await recipeIngredientRepository.DeleteByRecipeIdAsync(id, token);

        await base.DeleteAsync(id, token);

        scope.Complete();
    }


    private async Task ValidateIngredientsAsync(RecipeCreateDto request, CancellationToken token)
    {
        if (request.Ingredients == null || request.Ingredients.Length == 0)
            return;

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

    private async Task AddIngredientsToRecipeAsync(Guid recipeId, RecipeIngredientCreateDto[]? ingredients,
        CancellationToken token)
    {
        if (ingredients == null || ingredients.Length == 0)
            return;

        var links = ingredients
            .DistinctBy(x => x.IngredientId)
            .Select(i => new RecipeIngredient
            {
                RecipeId = recipeId,
                IngredientId = i.IngredientId,
                Quantity = i.Quantity
            });

        await recipeIngredientRepository.AddRangeAsync(links, token);
    }
}