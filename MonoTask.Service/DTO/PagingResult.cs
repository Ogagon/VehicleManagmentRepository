using System;
using System.Collections.Generic;

namespace MonoTask.Service.DTO
{
    public class PagingResult<T>
    {
        public List<T> Items { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
        public int TotalPages =>
            (int)Math.Ceiling((double)TotalItems / PageSize);
        public PagingResult(List<T> items, int totalItems, int currentPage, int pageSize)
        {
            Items = items;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalItems = totalItems;
        }
    }
}
