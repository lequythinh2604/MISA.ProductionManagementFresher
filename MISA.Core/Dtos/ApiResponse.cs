
using MISA.Core.Enums;

namespace MISA.Core.Dtos
{
    /// <summary>
    /// Response chuẩn cho API
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu trả về</typeparam>
    /// Created by: ThinhLQ (03/12/2025)
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public HttpStatusCode Code { get; set; }
        public T? Data { get; set; }
        public List<string> ValidateInfo { get; set; } = new List<string>();
        public string? UserMessage { get; set; }
        public string? SystemMessage { get; set; }
        public DateTime? ServerTime { get; set; }

        /// <summary>
        /// Tạo response thành công
        /// </summary>
        /// <param name="data">Dữ liệu trả về</param>
        /// <param name="code">status code</param>
        /// <param name="userMessage">Thông báo cho người dùng</param>
        /// <returns>ApiResponse với Success = true, Code = 200 (mặc định)</returns>
        /// Created by: ThinhLQ (03/12/2025)
        public static ApiResponse<T> SuccessResponse(T data, HttpStatusCode code = HttpStatusCode.OK, string? userMessage = null)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Code = code,
                Data = data,
                ValidateInfo = new List<string>(),
                UserMessage = userMessage,
                SystemMessage = null,
                ServerTime = DateTime.Now,
            };
        }

        /// <summary>
        /// Tạo response lỗi (HTTP 4xx, 5xx)
        /// </summary>
        /// <param name="userMessage">Thông báo lỗi cho người dùng</param>
        /// <param name="code">Mã lỗi HTTP (400, 404, 500...)</param>
        /// <param name="systemMessage">Thông báo hệ thống (cho dev)</param>
        /// <param name="validateInfo">Danh sách lỗi validation chi tiết</param>
        /// <returns>ApiResponse với Success = false, Code = 500 (mặc định)</returns>
        /// Created by: ThinhLQ (03/12/2025)
        public static ApiResponse<T> ErrorResponse(
            string userMessage,
            HttpStatusCode code = HttpStatusCode.InternalServerError,
            string? systemMessage = null,
            List<string>? validateInfo = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Code = code,
                Data = default(T),
                ValidateInfo = validateInfo ?? new List<string>(),
                UserMessage = userMessage,
                SystemMessage = systemMessage,
                ServerTime = DateTime.Now,
            };
        }
    }
}
