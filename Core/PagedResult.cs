// Core/PagedResult.cs
namespace Core;

/// <summary>
/// Generisk container til pagination.
/// Items = selve listen (en side), TotalCount = antal i alt, Page/PageSize = paging input.
/// </summary>
public record PagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int Page,
    int PageSize
);