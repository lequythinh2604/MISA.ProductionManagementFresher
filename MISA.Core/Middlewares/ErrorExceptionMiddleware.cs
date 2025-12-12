using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using MISA.Core.Dtos;
using MISA.Core.Enums;
using MISA.Core.Exceptions;
using Newtonsoft;

namespace MISA.Core.Middlewares
{
    /// <summary>
    /// Middleware xử lý các exception và chuyển đổi thành response chuẩn
    /// </summary>
    /// Created by: ThinhLQ (03/12/2025)
    public class ErrorExceptionMiddleware
    {
        private readonly RequestDelegate _delegate;

        /// <summary>
        /// Khởi tạo middleware
        /// </summary>
        /// <param name="delegate">Request delegate tiếp theo trong pipeline</param>
        /// Created by: ThinhLQ (03/12/2025)
        public ErrorExceptionMiddleware(RequestDelegate @delegate)
        {
            _delegate = @delegate;
        }

        /// <summary>
        /// Xử lý request và bắt các exception
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// Created by: ThinhLQ (03/12/2025)
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _delegate(context);
            }
            catch (MISAValidateException ex)
            {
                await HandleExceptionAsync(context, HttpStatusCode.BadRequest, ex.Message, "Validation Error", new List<string> { ex.Message });
            }
            catch (MISANotFoundException ex)
            {
                await HandleExceptionAsync(context, HttpStatusCode.NotFound, ex.Message, "Not Found Error");
            }
            catch (MISADuplicateException ex)
            {
                await HandleExceptionAsync(context, HttpStatusCode.Conflict, ex.Message, "Duplicate Error", new List<string>
                {
                    $"{ex.DuplicateField} {ex.ExistingName} đã tồn tại",
                });
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, ex.Message, "Đã xảy ra lỗi trong quá trình xử lý");
            }
        }

        private async Task HandleExceptionAsync(
            HttpContext context, 
            HttpStatusCode statusCode, 
            string userMessage, string? 
            systemMessage = null, 
            List<string>? validateInfo = null)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = ApiResponse<object>.ErrorResponse
            (
                userMessage: userMessage,
                code: statusCode,
                systemMessage: systemMessage,
                validateInfo: validateInfo
            );

            var jsonResponse = Newtonsoft.Json.JsonConvert.SerializeObject(response);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
