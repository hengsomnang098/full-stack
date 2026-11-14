namespace FullStack.Api.Application.DTOs.Common;

public class PagedResponse<T>
{
    public IReadOnlyList<T> List { get; set; } = Array.Empty<T>();
    public int Total { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(Total / (double)PageSize);
}
