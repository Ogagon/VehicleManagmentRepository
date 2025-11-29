using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoTask.Service.DTO
{
    public class PaginationRequest
    {
        public int Page { get; set; }
        public int PageSize { get; set; }

        public PaginationRequest(int page, int pageSize)
        {
            this.Page = page;
            this.PageSize = pageSize;
        }
    }
}
