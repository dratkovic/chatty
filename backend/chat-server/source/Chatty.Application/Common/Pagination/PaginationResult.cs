namespace Chatty.Application.Common.Pagination;

public class PaginationResult<T> where T : class
{
    public PaginationResult(List<T> data, int totalRecords, int paginationPage, int paginationPageSize)
    {
        Page = paginationPage;
        PageSize = paginationPageSize;
        TotalRecords = totalRecords;
        TotalPages = (int)Math.Ceiling(totalRecords / (double)PageSize);
        Data = data;
    }

    public int Page { get; private set; }
    public int PageSize { get; private set; }
    public int TotalPages { get; private set; }
    public int TotalRecords { get; private set; }
    public IReadOnlyList<T> Data { get; private set; }
}
