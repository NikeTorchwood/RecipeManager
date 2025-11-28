using Application.Contracts.Abstractions;

namespace Application.Contracts.Base;

public abstract class FilterBase<T> : IFilter<T>
{
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
}