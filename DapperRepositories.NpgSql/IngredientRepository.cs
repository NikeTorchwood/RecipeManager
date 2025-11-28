using System.Data;
using Application.Contracts.Base;
using Application.Contracts.Ingredients;
using Application.Repositories.Abstractions;
using Core;
using Dapper;
using DapperRepositories.NpgSql.Base;

namespace DapperRepositories.NpgSql;

public class IngredientRepository(IDbConnection connection) : RepositoryBase<Ingredient>(connection), IIngredientRepository
{
    protected override void ApplyFilters(SqlBuilder builder, FilterBase<Ingredient> filter)
    {
        if (filter is IngredientFilter f)
        {
            if (!string.IsNullOrWhiteSpace(f.Name))
            {
                builder.Where("\"Name\" ILIKE @Name", new { Name = $"%{f.Name}%" });
            }
        }
    }
}