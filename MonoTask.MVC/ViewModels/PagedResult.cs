using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MonoTask.MVC.ViewModels
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; }

        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
        public string CurrentSortColumn { get; set; } = "Name";
        public bool CurrentSortDescending { get; set; } = false;
        public string CurrentSearchTerm { get; set; }
        public SelectList SelectMakes { get; set; }
        public int? CurrentMakeId { get; set; }

        public int TotalPages =>
            (int)Math.Ceiling((double)TotalItems / PageSize);
    }

}