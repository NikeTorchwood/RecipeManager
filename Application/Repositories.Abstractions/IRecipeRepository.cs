using Application.Contracts.Recipes;
using Application.Repositories.Abstractions.Base;
using Core;

namespace Application.Repositories.Abstractions;

public interface IRecipeRepository : IRepository<Recipe>
{
    public Task<RecipeFullDto> GetFullByIdAsync(Guid id, CancellationToken token = default);
}