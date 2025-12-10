
using System.ComponentModel.DataAnnotations.Schema;

namespace MISA.Core.Entities
{
    [Table("work_shift")]
    public class WorkShift
    {
        [Column("work_shift_id")]
        public Guid WorkShiftId { get; set; }

        [Column("work_shift_code")]
        public string WorkShiftCode { get; set; } = string.Empty;

        [Column("work_shift_name")]
        public string WorkShiftName { get; set; } = string.Empty;

        [Column("start_time")]
        public DateTime StartTime { get; set; }

        [Column("end_time")]
        public DateTime EndTime { get; set; }

        [Column("break_start")]
        public DateTime? BreakStart { get; set; }

        [Column("break_end")]
        public DateTime? BreakEnd { get; set; }

        [Column("working_hours")]
        public decimal WorkingHours { get; set; }

        [Column("break_hours")]
        public decimal BreakHours { get; set; }

        [Column("work_shift_status")]
        public bool? WorkShiftStatus { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Column("created_by")]
        public string? CreatedBy { get; set; }

        [Column("modified_date")]
        public DateTime ModifiedDate { get; set; }

        [Column("modified_by")]
        public string? ModifiedBy { get; set; }

        [Column("description")]
        public string? Description { get; set; }
    }
}
