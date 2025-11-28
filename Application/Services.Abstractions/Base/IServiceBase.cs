using Application.Contracts.Abstractions;
using Application.Contracts.Base;
using Core.Abstractions;

namespace Application.Services.Abstractions.Base;

public interface IServiceBase<T, TShortDto, TFullDto, in TFilter, in TCreateDto>
    where T : class, IEntity
    where TShortDto : class, IShortDto<T>
    where TCreateDto : class, ICreateDto<T>
    where TFullDto : class, IFullDto<T>
    where TFilter : class, IFilter<T>
{
    public Task<PageResult<TShortDto>> GetAllAsync(TFilter filter, CancellationToken token = default);
    public Task<TFullDto?> GetByIdAsync(Guid id, CancellationToken token = default);
    public Task<Guid> CreateAsync(TCreateDto request, CancellationToken token = default);
    public Task UpdateAsync(Guid id, TCreateDto request, CancellationToken token = default);
    public Task DeleteAsync(Guid id, CancellationToken token = default);
}