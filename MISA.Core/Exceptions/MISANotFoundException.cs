using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.Exceptions
{
    /// <summary>
    /// Exception được throw khi không tìm thấy tài nguyên
    /// </summary>
    /// Created by: LQThinh (04/12/2025)
    public class MISANotFoundException : Exception
    {
        /// <summary>
        /// Khởi tạo exception với thông báo lỗi
        /// </summary>
        /// <param name="message">Thông báo lỗi</param>
        /// Created by: LQThinh (04/12/2025)
        public MISANotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        /// Khởi tạo exception với thông báo lỗi và exception gốc
        /// </summary>
        /// <param name="message">Thông báo lỗi</param>
        /// <param name="innerException">Exception gốc</param>
        /// Created by: LQThinh (04/12/2025)
        public MISANotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
