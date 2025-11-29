using Application.Repositories.Abstractions.Base;
using Core;

namespace Application.Repositories.Abstractions;

public interface IRecipeIngredientRepository: IRepository<RecipeIngredient>
{
    public Task<IEnumerable<RecipeIngredient>> GetByRecipeIdAsync(Guid recipeId,
        CancellationToken token = default);
    public Task DeleteByRecipeIdAsync(Guid id, CancellationToken token = default);
    public Task DeleteManyAsync(Guid[] toDeleteIds, CancellationToken token);
}