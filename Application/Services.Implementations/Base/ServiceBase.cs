using Application.Contracts.Abstractions;
using Application.Contracts.Base;
using Application.Repositories.Abstractions.Base;
using Application.Services.Abstractions.Base;
using AutoMapper;
using Core.Abstractions;

namespace Application.Services.Implementations.Base;

public abstract class ServiceBase<T, TShortDto, TFullDto, TFilter, TCreateDto>(
    IRepository<T> repository,
    IMapper mapper)
    : IServiceBase<
        T,
        TShortDto,
        TFullDto,
        TFilter,
        TCreateDto>
    where T : class, IEntity
    where TShortDto : class, IShortDto<T>
    where TFullDto : class, IFullDto<T>
    where TFilter : FilterBase<T>
    where TCreateDto : class, ICreateDto<T>
{
    public virtual async Task<PageResult<TShortDto>> GetAllAsync(TFilter filter, CancellationToken token = default)
    {
        var items = await repository.GetAllProjectedAsync<TShortDto>(filter, token);

        var count = await repository.CountAsync(filter, token);

        return items.ToPageResult(filter.PageNumber, filter.PageSize, count);
    }

    public virtual async Task<TFullDto?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        var entity = await repository.GetByIdAsync(id, token);
        return mapper.Map<TFullDto?>(entity);
    }

    public virtual async Task<Guid> CreateAsync(TCreateDto request, CancellationToken token = default)
    {
        var entity = mapper.Map<T>(request);

        return await repository.AddAsync(entity, token);
    }

    public virtual async Task UpdateAsync(Guid id, TCreateDto request, CancellationToken token = default)
    {
        var existingEntity = await repository.GetByIdAsync(id, token);

        if (existingEntity == null)
            throw new KeyNotFoundException($"Entity {typeof(T).Name} with id {id} not found");

        mapper.Map(request, existingEntity);

        await repository.UpdateAsync(existingEntity, token);
    }

    public virtual async Task DeleteAsync(Guid id, CancellationToken token = default)
    {
        var deleted = await repository.DeleteAsync(id, token);

        if (!deleted)
            throw new KeyNotFoundException($"Entity {typeof(T).Name} with id {id} not found");
    }
}