﻿namespace Chatty.Application.Common.Pagination;

public class PaginationQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Filter { get; set; }
}
