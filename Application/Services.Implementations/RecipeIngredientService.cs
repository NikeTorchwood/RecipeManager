using Application.Contracts.RecipeIngredients;
using Application.Exceptions;
using Application.Repositories.Abstractions;
using Application.Services.Abstractions;
using Application.Services.Implementations.Base;
using AutoMapper;
using Core;

namespace Application.Services.Implementations;

public class RecipeIngredientService(
    IRecipeRepository recipeRepository,
    IIngredientRepository ingredientRepository,
    IRecipeIngredientRepository repository,
    IMapper mapper)
    : ServiceBase<
            RecipeIngredient,
            RecipeIngredientShortDto,
            RecipeIngredientFullDto,
            RecipeIngredientFilter,
            RecipeIngredientCreateDto>(repository, mapper),
        IRecipeIngredientService
{
    private readonly IMapper _mapper = mapper;

    public override async Task<Guid> CreateAsync(RecipeIngredientCreateDto request, CancellationToken token = default)
    {
        if (!await recipeRepository.ExistsAsync(request.RecipeId, token))
            throw new BadRequestException("Recipe doesn't exist");
        if (!await ingredientRepository.ExistsAsync(request.IngredientId, token))
            throw new BadRequestException("Ingredient doesn't exist");

        var existingLink = await repository.FindLinkAsync(request.RecipeId, request.IngredientId, token);
        if (existingLink is not null)
            throw new BadRequestException("Can't add recipe duplicate link");

        return await base.CreateAsync(request, token);
    }

    public override async Task UpdateAsync(Guid id, RecipeIngredientCreateDto request,
        CancellationToken token = default)
    {
        var currentLink = await repository.GetByIdAsync(id, token);
        if (currentLink == null)
            throw new KeyNotFoundException($"RecipeIngredient link with id {id} not found.");

        if (request.RecipeId != currentLink.RecipeId)
        {
            if (!await recipeRepository.ExistsAsync(request.RecipeId, token))
                throw new BadRequestException($"Target Recipe {request.RecipeId} not found.");
        }

        if (request.IngredientId != currentLink.IngredientId)
        {
            if (!await ingredientRepository.ExistsAsync(request.IngredientId, token))
                throw new ArgumentException($"Target Ingredient {request.IngredientId} not found.");
        }

        var result = _mapper.Map(request, currentLink);

        await repository.UpdateAsync(result, token);
    }
}