using System.Data;
using Application.Contracts.Base;
using Application.Contracts.Recipes;
using Application.Repositories.Abstractions;
using Core;
using Dapper;
using DapperRepositories.NpgSql.Base;

namespace DapperRepositories.NpgSql;

public class RecipeRepository(IDbConnection connection)
    : RepositoryBase<Recipe>(connection), IRecipeRepository
{
    private readonly IDbConnection _connection = connection;

    protected override void ApplyFilters(SqlBuilder builder, FilterBase<Recipe> filter)
    {
        if (filter is RecipeFilter f)
        {
            if (!string.IsNullOrWhiteSpace(f.Name))
                builder.Where("r.\"Name\" ILIKE @Name", new { Name = $"%{f.Name}%" });

            if (!string.IsNullOrWhiteSpace(f.Description))
                builder.Where("r.\"Description\" ILIKE @Desc", new { Desc = $"%{f.Description}%" });

            if (f.CategoryId.HasValue)
                builder.Where("r.\"CategoryId\" = @CatId", new { CatId = f.CategoryId });

            if (f.CreatedFrom.HasValue)
                builder.Where("r.\"CreatedAt\" >= @CreatedFrom", new { CreatedFrom = f.CreatedFrom });

            if (f.CreatedTo.HasValue)
                builder.Where("r.\"CreatedAt\" <= @CreatedTo", new { CreatedTo = f.CreatedTo });

            if (f.IngredientId.HasValue)
            {
                builder.Where(@"EXISTS (
                    SELECT 1 FROM ""RecipeIngredients"" ri 
                    WHERE ri.""RecipeId"" = r.""Id"" 
                    AND ri.""IngredientId"" = @IngFilterId
                )", new { IngFilterId = f.IngredientId });
            }
        }
    }

    public async Task<RecipeFullDto> GetFullByIdAsync(Guid id, CancellationToken token = default)
    {
        var sql = @"
        SELECT 
            r.""Id"", 
            r.""Name"", 
            r.""Description"", 
            r.""CategoryId"", 
            r.""CreatedAt"", 
            c.""Name"" as ""CategoryName""
        FROM ""Recipes"" r
        LEFT JOIN ""Categories"" c ON r.""CategoryId"" = c.""Id""
        WHERE r.""Id"" = @Id;

        SELECT 
            ri.""Id"", 
            ri.""IngredientId"", 
            i.""Name"", 
            ri.""Quantity""
        FROM ""RecipeIngredients"" ri
        JOIN ""Ingredients"" i ON ri.""IngredientId"" = i.""Id""
        WHERE ri.""RecipeId"" = @Id;";

        await using var multi = await _connection.QueryMultipleAsync(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: token));

        var recipeDto = await multi.ReadSingleOrDefaultAsync<RecipeFullDto>();

        if (recipeDto != null)
        {
            var ingredients = await multi.ReadAsync<RecipeIngredientDto>();
            recipeDto.Ingredients = ingredients.ToList();
        }

        return recipeDto;
    }

    public override async Task<IEnumerable<TShortDto>> GetAllProjectedAsync<TShortDto>(
        FilterBase<Recipe> filter, CancellationToken token = default)
    {
        var builder = new SqlBuilder();
        var dtoProps = typeof(TShortDto).GetProperties();

        foreach (var prop in dtoProps)
        {
            builder.Select($"r.\"{prop.Name}\"");
        }

        ApplyFilters(builder, filter);

        var sortClause = GenerateSortClause(filter.SortColumn, filter.SortDescending, tableAlias: "r");

        var template = builder.AddTemplate($@"
        SELECT /**select**/ 
        FROM ""{TableName}"" r
        LEFT JOIN ""{Constats.Categories}"" c ON r.""CategoryId"" = c.""Id""
        /**where**/
        {sortClause}
        LIMIT @PageSize OFFSET @Offset",
            new
            {
                PageSize = filter.PageSize < 1 ? 10 : filter.PageSize,
                Offset = ((filter.PageNumber < 1 ? 1 : filter.PageNumber) - 1) * filter.PageSize
            });

        return await _connection.QueryAsync<TShortDto>(
            new CommandDefinition(template.RawSql, template.Parameters, cancellationToken: token));
    }
}