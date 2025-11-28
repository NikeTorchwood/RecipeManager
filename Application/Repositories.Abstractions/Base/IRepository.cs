using Application.Contracts.Abstractions;
using Application.Contracts.Base;
using Core.Abstractions;

namespace Application.Repositories.Abstractions.Base;

public interface IRepository<T>
    where T : class, IEntity
{
    public Task<T?> GetByIdAsync(Guid id, CancellationToken token = default);
    public Task<bool> ExistsAsync(Guid id, CancellationToken token = default);
    public Task<Guid[]> ExistingIdsAsync(Guid[] ids, CancellationToken token = default);
    public Task UpdateAsync(T entity, CancellationToken token = default);

    public Task<IEnumerable<TShortDto>> GetAllProjectedAsync<TShortDto>(FilterBase<T> filter,
        CancellationToken token = default) where TShortDto : class, IShortDto<T>;

    public Task<int> CountAsync(FilterBase<T> filter, CancellationToken token = default);
    public Task<Guid> AddAsync(T entity, CancellationToken token = default);
    public Task AddRangeAsync(IEnumerable<T> links, CancellationToken token = default);
    public Task<bool> DeleteAsync(Guid id, CancellationToken token = default);
}