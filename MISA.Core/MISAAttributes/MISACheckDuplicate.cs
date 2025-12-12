using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.MISAAttributes
{
    /// <summary>
    /// Attribute đánh dấu property cần kiểm tra trùng lặp
    /// </summary>
    /// Created by: ThinhLQ (10/12/2025)
    [AttributeUsage(AttributeTargets.Property)]
    public class MISACheckDuplicate : Attribute
    {
        public string ErrorMessage { get; set; }
        public MISACheckDuplicate(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
    }
}
