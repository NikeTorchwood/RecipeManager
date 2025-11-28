using System.Data;
using Application.Contracts.Base;
using Application.Contracts.Ingredients;
using Application.Contracts.Recipes;
using Application.Repositories.Abstractions;
using Core;
using Dapper;
using DapperRepositories.NpgSql.Base;

namespace DapperRepositories.NpgSql;

public class RecipeRepository(IDbConnection connection) 
    : RepositoryBase<Recipe>(connection), IRecipeRepository
{
    protected override void ApplyFilters(SqlBuilder builder, FilterBase<Recipe> filter)
    {
        if (filter is RecipeFilter f)
        {
            if (!string.IsNullOrWhiteSpace(f.Name))
            {
                builder.Where("\"Name\" ILIKE @Name", new { Name = $"%{f.Name}%" });
            }
            
            if (!string.IsNullOrWhiteSpace(f.Description))
            {
                builder.Where("\"Description\" ILIKE @Desc", new { Desc = $"%{f.Description}%" });
            }
            
            if (f.CategoryId.HasValue)
            {
                builder.Where("\"CategoryId\" = @CatId", new { CatId = f.CategoryId });
            }
            
            // Фильтр по дате (если передан)
            // Обычно ищут диапазон, но тут точное или "больше чем"
            if (f.CreatedAt.HasValue)
            {
                builder.Where("\"CreatedAt\" >= @Created", new { Created = f.CreatedAt });
            }
        }
    }

    public async Task<RecipeFullDto> GetFullByIdAsync(Guid id, CancellationToken token = default)
    {
        var sql = @"
            SELECT r.""Id"", r.""Name"", r.""Description"", r.""CategoryId"", r.""CreatedAt"", c.""Name"" as ""CategoryName""
            FROM ""Recipes"" r
            LEFT JOIN ""Categories"" c ON r.""CategoryId"" = c.""Id""
            WHERE r.""Id"" = @Id;

            SELECT i.""Name"", ri.""Quantity""
            FROM ""RecipeIngredients"" ri
            JOIN ""Ingredients"" i ON ri.""IngredientId"" = i.""Id""
            WHERE ri.""RecipeId"" = @Id;";

        await using var multi = await connection.QueryMultipleAsync(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: token));

        var recipeDto = await multi.ReadSingleOrDefaultAsync<RecipeFullDto>();

        if (recipeDto != null)
        {
            var ingredients = await multi.ReadAsync<IngredientShortDto>();
            recipeDto.Ingredients = ingredients.ToList();
        }

        return recipeDto;
    }
    
    public override async Task<IEnumerable<TShortDto>> GetAllProjectedAsync<TShortDto>(
        FilterBase<Recipe> filter, CancellationToken token = default)
    {
        var builder = new SqlBuilder();
        
        builder.Select("r.\"Id\", r.\"Name\", r.\"CreatedAt\""); 
        
        if (typeof(TShortDto).GetProperty("CategoryName") != null)
        {
            builder.Select("c.\"Name\" as \"CategoryName\"");
        }

        ApplyFilters(builder, filter);

        var template = builder.AddTemplate($@"
            SELECT /**select**/ 
            FROM ""Recipes"" r
            LEFT JOIN ""Categories"" c ON r.""CategoryId"" = c.""Id""
            /**where**/
            ORDER BY r.""CreatedAt"" DESC
            LIMIT @PageSize OFFSET @Offset",
            new 
            { 
                PageSize = filter.PageSize, 
                Offset = (filter.PageNumber - 1) * filter.PageSize 
            });

        return await connection.QueryAsync<TShortDto>(
            new CommandDefinition(template.RawSql, template.Parameters, cancellationToken: token));
    }
}