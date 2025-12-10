using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.Dtos
{
    public class WorkShiftDeleteDto
    {
        public HashSet<Guid> workShiftIds { get; set; } = new HashSet<Guid>();
    }
}
