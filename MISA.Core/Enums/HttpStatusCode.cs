using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.Enums
{
    /// <summary>
    /// Các mã trạng thái HTTP
    /// </summary>
    public enum HttpStatusCode
    {
        /// <summary>
        /// Successful
        /// </summary>
        OK = 200, // Yêu cầu thành công
        Created = 201, // Yêu cầu đã thành công và một tài nguyên mới đã được tạo.
        NoContent = 204, // Yêu cầu đã được xử lý thành công nhưng không có nội dung để trả về.
        /// <summary>
        /// Client Error
        /// </summary>
        BadRequest = 400, // Yêu cầu có cú pháp sai hoặc không thể được đáp ứng.
        NotFound = 404, // Không tìm thấy tài nguyên được yêu cầu.
        Conflict = 409, // Xung đột xảy ra, thường do trùng lặp dữ liệu.
        /// <summary>
        /// Server Error
        /// </summary>
        InternalServerError = 500,
    }
}
