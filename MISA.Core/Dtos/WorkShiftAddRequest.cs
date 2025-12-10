using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.Dtos
{
    public class WorkShiftAddRequest
    {
        public string? WorkShiftCode { get; set; }
        public string? WorkShiftName { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? BreakStart { get; set; }
        public DateTime? BreakEnd { get; set; }
        public decimal WorkingHours { get; set; }
        public decimal BreakHours { get; set; }
        public Boolean WorkShiftStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public string? Description { get; set; } 
    }
}
