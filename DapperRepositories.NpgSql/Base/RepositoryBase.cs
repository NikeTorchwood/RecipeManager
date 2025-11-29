using System.Collections.Concurrent;
using System.Data;
using System.Reflection;
using Application.Contracts.Abstractions;
using Application.Contracts.Base;
using Application.Repositories.Abstractions.Base;
using Core.Abstractions;
using Dapper;
using Dapper.Contrib.Extensions;

namespace DapperRepositories.NpgSql.Base;

public abstract class RepositoryBase<T>(IDbConnection connection) : IRepository<T>
    where T : class, IEntity
{
    protected static readonly string TableName = GetTableName();

    private static readonly IEnumerable<PropertyInfo> EntityProperties = typeof(T).GetProperties()
        .Where(x => x.Name != "Id" && x.CanRead)
        .ToList();

    private static readonly string InsertSql = GenerateInsertSql();
    private static readonly string BulkInsertSql = GenerateBulkInsertSql();
    private static readonly string UpdateSql = GenerateUpdateSql();

    private static readonly ConcurrentDictionary<Type, string> _projectionCache = new();

    private static readonly Dictionary<string, string> SafeSortColumns =
        typeof(T).GetProperties()
            .ToDictionary(p => p.Name.ToLowerInvariant(), p => p.Name);

    private static string GetTableName()
    {
        var type = typeof(T);
        var tableAttr = type.GetCustomAttribute<TableAttribute>();
        return tableAttr != null ? tableAttr.Name : type.Name;
    }

    private static string GenerateInsertSql()
    {
        var columns = string.Join(", ", EntityProperties.Select(p => $"\"{p.Name}\""));
        var paramsNames = string.Join(", ", EntityProperties.Select(p => "@" + p.Name));
        return $"INSERT INTO \"{TableName}\" ({columns}) VALUES ({paramsNames}) RETURNING \"Id\"";
    }

    private static string GenerateBulkInsertSql()
    {
        var columns = string.Join(", ", EntityProperties.Select(p => $"\"{p.Name}\""));
        var paramsNames = string.Join(", ", EntityProperties.Select(p => "@" + p.Name));
        return $"INSERT INTO \"{TableName}\" ({columns}) VALUES ({paramsNames})";
    }

    private static string GenerateUpdateSql()
    {
        var setClause = string.Join(", ", EntityProperties.Select(p => $"\"{p.Name}\" = @{p.Name}"));
        return $"UPDATE \"{TableName}\" SET {setClause} WHERE \"Id\" = @Id";
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        var sql = $"SELECT * FROM \"{TableName}\" WHERE \"Id\" = @Id";
        return await connection.QuerySingleOrDefaultAsync<T>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: token));
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken token = default)
    {
        var sql = $"SELECT 1 FROM \"{TableName}\" WHERE \"Id\" = @Id LIMIT 1";
        return await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: token));
    }

    public async Task<Guid[]> ExistingIdsAsync(Guid[] ids, CancellationToken token = default)
    {
        if (ids.Length == 0) return [];
        var sql = $"SELECT \"Id\" FROM \"{TableName}\" WHERE \"Id\" = ANY(@Ids)";
        return (await connection.QueryAsync<Guid>(
                new CommandDefinition(sql, new { Ids = ids }, cancellationToken: token)))
            .ToArray();
    }

    public async Task UpdateAsync(T entity, CancellationToken token = default)
    {
        await connection.ExecuteAsync(
            new CommandDefinition(UpdateSql, entity, cancellationToken: token));
    }

    public async Task<Guid> AddAsync(T entity, CancellationToken token = default)
    {
        return await connection.QuerySingleAsync<Guid>(
            new CommandDefinition(InsertSql, entity, cancellationToken: token));
    }

    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken token = default)
    {
        await connection.ExecuteAsync(
            new CommandDefinition(BulkInsertSql, entities, cancellationToken: token));
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken token = default)
    {
        var sql = $"DELETE FROM \"{TableName}\" WHERE \"Id\" = @Id";
        var rows = await connection.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: token));
        return rows > 0;
    }

    public virtual async Task<int> CountAsync(FilterBase<T> filter, CancellationToken token = default)
    {
        var builder = new SqlBuilder();
        ApplyFilters(builder, filter);
        var template = builder.AddTemplate($"SELECT COUNT(*) FROM \"{TableName}\" /**where**/");

        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(template.RawSql, template.Parameters, cancellationToken: token));
    }

    public virtual async Task<IEnumerable<TShortDto>> GetAllProjectedAsync<TShortDto>(
        FilterBase<T> filter, CancellationToken token = default)
        where TShortDto : class, IShortDto<T>
    {
        var selectColumns = _projectionCache.GetOrAdd(typeof(TShortDto), type =>
        {
            var props = type.GetProperties();
            return string.Join(", ", props.Select(p => $"\"{p.Name}\""));
        });

        var builder = new SqlBuilder();

        ApplyFilters(builder, filter);

        var sortClause = GenerateSortClause(filter.SortColumn, filter.SortDescending);

        var selector = builder.AddTemplate(
            $@"SELECT {selectColumns} FROM ""{TableName}""
           /**where**/  
           {sortClause} 
           LIMIT @PageSize OFFSET @Offset",
            new
            {
                PageSize = filter.PageSize < 1 ? 10 : filter.PageSize,
                Offset = ((filter.PageNumber < 1 ? 1 : filter.PageNumber) - 1) * filter.PageSize
            });

        return await connection.QueryAsync<TShortDto>(
            new CommandDefinition(selector.RawSql, selector.Parameters, cancellationToken: token));
    }

    protected virtual void ApplyFilters(SqlBuilder builder, FilterBase<T> filter)
    {
    }

    protected string GenerateSortClause(string? columnName, bool descending, string tableAlias = "")
    {
        if (string.IsNullOrWhiteSpace(columnName) ||
            !SafeSortColumns.TryGetValue(columnName.ToLowerInvariant(), out var realColumnName))
        {
            realColumnName = "Id";
        }

        var direction = descending ? "DESC" : "ASC";
        var prefix = string.IsNullOrEmpty(tableAlias) ? "" : $"{tableAlias}.";

        return $"ORDER BY {prefix}\"{realColumnName}\" {direction}";
    }
}