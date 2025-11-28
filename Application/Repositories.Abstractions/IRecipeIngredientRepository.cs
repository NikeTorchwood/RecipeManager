using Application.Repositories.Abstractions.Base;
using Core;

namespace Application.Repositories.Abstractions;

public interface IRecipeIngredientRepository : IRepository<RecipeIngredient>
{
    public Task<RecipeIngredient?> FindLinkAsync(Guid requestRecipeId, Guid requestIngredientId,
        CancellationToken token = default);

    public Task DeleteByRecipeIdAsync(Guid id, CancellationToken token = default);
}