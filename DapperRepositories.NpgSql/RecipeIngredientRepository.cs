using System.Data;
using Application.Contracts.Base;
using Application.Contracts.RecipeIngredients;
using Application.Repositories.Abstractions;
using Core;
using Dapper;
using DapperRepositories.NpgSql.Base;

namespace DapperRepositories.NpgSql;

public class RecipeIngredientRepository(IDbConnection connection) 
    : RepositoryBase<RecipeIngredient>(connection), IRecipeIngredientRepository
{
    protected override void ApplyFilters(SqlBuilder builder, FilterBase<RecipeIngredient> filter)
    {
        if (filter is RecipeIngredientFilter f)
        {
            if (f.RecipeId.HasValue)
            {
                builder.Where("\"RecipeId\" = @RId", new { RId = f.RecipeId });
            }
            
            if (f.IngredientId.HasValue)
            {
                builder.Where("\"IngredientId\" = @IId", new { IId = f.IngredientId });
            }
            
            if (!string.IsNullOrWhiteSpace(f.Quantity))
            {
                builder.Where("\"Quantity\" ILIKE @Qty", new { Qty = $"%{f.Quantity}%" });
            }
        }
    }

    public async Task<RecipeIngredient?> FindLinkAsync(Guid requestRecipeId, Guid requestIngredientId, CancellationToken token = default)
    {
        var sql = $"SELECT * FROM \"{TableName}\" WHERE \"RecipeId\" = @RId AND \"IngredientId\" = @IId LIMIT 1";
        
        return await connection.QuerySingleOrDefaultAsync<RecipeIngredient>(
            new CommandDefinition(sql, new { RId = requestRecipeId, IId = requestIngredientId }, cancellationToken: token));
    }

    public async Task DeleteByRecipeIdAsync(Guid id, CancellationToken token = default)
    {
        var sql = $"DELETE FROM \"{TableName}\" WHERE \"RecipeId\" = @Id";
        
        await connection.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: token));
    }
}