using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.Dtos
{
    public class ColumnFilter
    {
        public string? ColumnName { get; set; }
        public string? FilterOperator { get; set; }
        public object? FilterValue { get; set; }
    }
}
