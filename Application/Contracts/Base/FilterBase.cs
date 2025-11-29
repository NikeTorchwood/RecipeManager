using Application.Contracts.Abstractions;

namespace Application.Contracts.Base;

public abstract class FilterBase<T> : IFilter<T>
{
    public int PageSize { get; init; } = 10;
    public int PageNumber { get; init; } = 1;
    public string? SortColumn { get; init; } = string.Empty;
    public bool SortDescending { get; init; }
}