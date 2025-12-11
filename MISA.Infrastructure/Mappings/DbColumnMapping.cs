using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Infrastructure.Mappings
{
    /// <summary>
    /// Chứa các ánh xạ tên cột sang tên cột trong db
    /// </summary>
    /// Created by: LQThinh (10/12/2025)
    public class DbColumnMapping
    {
        /// <summary>
        /// Ánh xạ cột cho Entity WorkShift.
        /// Key: Entities
        /// Value: work_shift db
        /// </summary>
        /// Created by: LQThinh (10/12/2025)
        public static readonly IDictionary<string, string> WorkShiftMapping =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "workShiftId", "work_shift_id" },
            { "workShiftCode", "work_shift_code" },
            { "workShiftName", "work_shift_name" },
            { "startTime", "start_time" },
            { "endTime", "end_time" },
            { "breakStart", "break_start" },
            { "breakEnd", "break_end" },
            { "workingHours", "working_hours" },
            { "breakHours", "break_hours" },
            { "workShiftStatus", "work_shift_status" },
            { "description", "description" },
            { "createdDate", "created_date" },
            { "createdBy", "created_by" },
            { "modifiedDate", "modified_date" },
            { "modifiedBy", "modified_by" },
        };
    }
}
