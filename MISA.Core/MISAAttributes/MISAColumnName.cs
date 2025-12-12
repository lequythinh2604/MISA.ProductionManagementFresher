using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.MISAAttributes
{
    /// <summary>
    /// Attribute chỉ định tên cột tương ứng trong database
    /// </summary>
    /// Created by: ThinhLQ (10/12/2025)
    [AttributeUsage(AttributeTargets.Property)]
    public class MISAColumnName : Attribute
    {
        public string ColumnName { get; set; }
        public MISAColumnName(string columnName)
        {
            ColumnName = columnName;
        }
    }
}
