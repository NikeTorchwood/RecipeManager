using System.Data;
using System.Reflection;
using Application.Contracts.Abstractions;
using Application.Contracts.Base;
using Application.Repositories.Abstractions.Base;
using Core;
using Core.Abstractions;
using Dapper;

namespace DapperRepositories.NpgSql.Base;

public abstract class RepositoryBase<T>(IDbConnection connection) : IRepository<T> where T : class, IEntity
{
    protected static readonly string TableName = Constats.TableNames[typeof(T).Name];

    private static readonly IEnumerable<PropertyInfo> Properties = typeof(T).GetProperties()
        .Where(x => x.Name != "Id" && x.CanRead)
        .ToList();

    private static readonly string InsertSql = GenerateInsertSql();
    private static readonly string UpdateSql = GenerateUpdateSql();
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
        => await connection.ExecuteAsync(
            new CommandDefinition(UpdateSql, entity, cancellationToken: token));

public virtual async Task<IEnumerable<TShortDto>> GetAllProjectedAsync<TShortDto>(
        FilterBase<T> filter, CancellationToken token = default) 
        where TShortDto : class, IShortDto<T>
    {
        var builder = new SqlBuilder();

        var dtoProps = typeof(TShortDto).GetProperties();
        foreach (var prop in dtoProps)
        {
            builder.Select($"\"{prop.Name}\"");
        }
        
        ApplyFilters(builder, filter);
        
        var template = builder.AddTemplate($@"
            SELECT /**select**/ FROM ""{TableName}"" 
            /**where**/ 
            ORDER BY ""Id"" 
            LIMIT @PageSize OFFSET @Offset", 
            new 
            { 
                PageSize = filter.PageSize, 
                Offset = (filter.PageNumber - 1) * filter.PageSize 
            });

        return await connection.QueryAsync<TShortDto>(
            new CommandDefinition(template.RawSql, template.Parameters, cancellationToken: token));
    }

    public virtual async Task<int> CountAsync(FilterBase<T> filter, CancellationToken token = default)
    {
        var builder = new SqlBuilder();

        ApplyFilters(builder, filter);
        
        var template = builder.AddTemplate($"SELECT COUNT(*) FROM \"{TableName}\" /**where**/");

        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(template.RawSql, template.Parameters, cancellationToken: token));
    }

    protected abstract void ApplyFilters(SqlBuilder builder, FilterBase<T> filter);

    public async Task<Guid> AddAsync(T entity, CancellationToken token = default) =>
        await connection.QuerySingleAsync<Guid>(
            new CommandDefinition(InsertSql, entity, cancellationToken: token));

    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken token = default)
    {
        var columns = string.Join(", ", Properties.Select(p => $"\"{p.Name}\""));
        var paramsNames = string.Join(", ", Properties.Select(p => "@" + p.Name));
        
        var sql = $"INSERT INTO \"{TableName}\" ({columns}) VALUES ({paramsNames})";

        await connection.ExecuteAsync(
            new CommandDefinition(sql, entities, cancellationToken: token));
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken token = default)
    {
        var sql = $"DELETE FROM \"{TableName}\" WHERE \"Id\" = @Id";
        
        var rows = await connection.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: token));
        
        return rows > 0;
    }
    
    private static string GenerateInsertSql()
    {
        var columns = string.Join(", ", Properties.Select(p => $"\"{p.Name}\""));
        var paramsNames = string.Join(", ", Properties.Select(p => "@" + p.Name));
        
        return $"INSERT INTO \"{TableName}\" ({columns}) VALUES ({paramsNames}) RETURNING \"Id\"";
    }

    private static string GenerateUpdateSql()
    {
        var setClause = string.Join(", ", Properties.Select(p => $"\"{p.Name}\" = @{p.Name}"));
        
        return $"UPDATE \"{TableName}\" SET {setClause} WHERE \"Id\" = @Id";
    }
}