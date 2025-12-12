using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.MISAAttributes
{
    /// <summary>
    /// Attribute chỉ định tên bảng tương ứng trong database
    /// </summary>
    /// Created by: ThinhLQ (10/12/2025)
    [AttributeUsage(AttributeTargets.Class)]
    public class MISATableName : Attribute
    {
        public string TableName { get; set; }
        public MISATableName(string tableName)
        {
            TableName = tableName;
        }
    }
}
