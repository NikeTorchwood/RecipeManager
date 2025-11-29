using System.Data;
using Application.Repositories.Abstractions;
using Core;
using Dapper;
using DapperRepositories.NpgSql.Base;

namespace DapperRepositories.NpgSql;

public class RecipeIngredientRepository(IDbConnection connection)
    : RepositoryBase<RecipeIngredient>(connection), IRecipeIngredientRepository
{
    private readonly IDbConnection _connection = connection;

    public async Task<IEnumerable<RecipeIngredient>> GetByRecipeIdAsync(Guid recipeId,
        CancellationToken token = default)
    {
        var sql = $"SELECT * FROM \"{TableName}\" WHERE \"RecipeId\" = @Id";

        return await _connection.QueryAsync<RecipeIngredient>(
            new CommandDefinition(sql, new { Id = recipeId }, cancellationToken: token));
    }

    public async Task<RecipeIngredient?> FindLinkAsync(Guid requestRecipeId, Guid requestIngredientId,
        CancellationToken token = default)
    {
        var sql = $"SELECT * FROM \"{TableName}\" WHERE \"RecipeId\" = @RId AND \"IngredientId\" = @IId LIMIT 1";

        return await _connection.QuerySingleOrDefaultAsync<RecipeIngredient>(
            new CommandDefinition(sql, new { RId = requestRecipeId, IId = requestIngredientId },
                cancellationToken: token));
    }

    public async Task DeleteByRecipeIdAsync(Guid id, CancellationToken token = default)
    {
        var sql = $"DELETE FROM \"{TableName}\" WHERE \"RecipeId\" = @Id";

        await _connection.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: token));
    }

    public async Task DeleteManyAsync(Guid[] toDeleteIds, CancellationToken token)
    {
        if (toDeleteIds.Length == 0) return;

        var sql = $"DELETE FROM \"{TableName}\" WHERE \"Id\" = ANY(@Ids)";

        await _connection.ExecuteAsync(
            new CommandDefinition(sql, new { Ids = toDeleteIds }, cancellationToken: token));
    }
}