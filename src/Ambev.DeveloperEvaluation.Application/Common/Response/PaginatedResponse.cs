﻿namespace Ambev.DeveloperEvaluation.Application.Common.Response;

public class PaginatedResponse <T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
}
