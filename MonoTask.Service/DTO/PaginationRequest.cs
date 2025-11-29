using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoTask.Service.DTO
{
    public class PaginationRequest
    {
        public int page { get; set; } = 1;
        public int pageSize { get; set; } = 10;
    }
}
