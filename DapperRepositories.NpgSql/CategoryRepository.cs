using System.Data;
using Application.Contracts.Base;
using Application.Contracts.Categories;
using Application.Repositories.Abstractions;
using Core;
using Dapper;
using DapperRepositories.NpgSql.Base;

namespace DapperRepositories.NpgSql;

public class CategoryRepository(IDbConnection connection) : RepositoryBase<Category>(connection), ICategoryRepository
{
    protected override void ApplyFilters(SqlBuilder builder, FilterBase<Category> filter)
    {
        if (filter is CategoryFilter f)
        {
            if (!string.IsNullOrWhiteSpace(f.Name))
            {
                builder.Where("\"Name\" ILIKE @Name", new { Name = $"%{f.Name}%" });
            }
        }
    }
}