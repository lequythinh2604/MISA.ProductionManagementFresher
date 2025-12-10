using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.Dtos
{
    public class WorkShiftFilterRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? Keyword { get; set; }
        public List<ColumnSort> SortColumn { get; set; } = new List<ColumnSort>();
        public List<ColumnFilter> FilterColumn { get; set; } = new List<ColumnFilter>();
    }
}
