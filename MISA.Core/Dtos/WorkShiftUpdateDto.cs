using MISA.Core.MISAAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.Dtos
{
    /// <summary>
    /// DTO cập nhật thông tin ca làm việc
    /// </summary>
    /// Created by: ThinhLQ (04/12/2025)
    public class WorkShiftUpdateDto
    {
        [MISARequired("Không được để trống")]
        [MISAColumnName("work_shift_code")]
        [MISACheckDuplicate("Mã ca không được trùng")]
        public string? WorkShiftCode { get; set; }

        [MISARequired("Không được để trống")]
        [MISAColumnName("work_shift_name")]
        public string? WorkShiftName { get; set; }

        [MISARequired("Không được để trống")]
        [MISAColumnName("start_time")]
        public DateTime? StartTime { get; set; }

        [MISARequired("Không được để trống")]
        [MISAColumnName("end_time")]
        public DateTime? EndTime { get; set; }

        [MISAColumnName("break_start")]
        public DateTime? BreakStart { get; set; }

        [MISAColumnName("break_end")]
        public DateTime? BreakEnd { get; set; }

        [MISAColumnName("working_hours")]
        public decimal WorkingHours { get; set; }

        [MISAColumnName("break_hours")]
        public decimal BreakHours { get; set; }

        [MISAColumnName("work_shift_status")]
        public bool? WorkShiftStatus { get; set; }

        [MISAColumnName("modified_date")]
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        [MISAColumnName("modified_by")]
        public string? ModifiedBy { get; set; } = "ThinhLQ";

        [MISAColumnName("description")]
        public string? Description { get; set; }
    }
}
