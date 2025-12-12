using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.Dtos
{
    public class StatusUpdateDto
    {
        public HashSet<Guid> ids { get; set; } = new HashSet<Guid>();
        public bool newStatus { get; set; }
    }
}
