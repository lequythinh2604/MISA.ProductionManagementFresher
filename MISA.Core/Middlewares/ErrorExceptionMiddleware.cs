using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using MISA.Core.Dtos;
using MISA.Core.Enums;
using MISA.Core.Exceptions;
using Newtonsoft;

namespace MISA.Core.Middlewares
{
    public class ErrorExceptionMiddleware
    {
        RequestDelegate _delegate;
        public ErrorExceptionMiddleware(RequestDelegate @delegate)
        {
            _delegate = @delegate;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _delegate(context);
            }
            catch (MISAValidateException ex)
            {
                var response = new ApiResponse<object>
                {
                    Code = HttpStatusCode.BadRequest,
                    SystemMessage = ex.Message,
                    Data = ex.Errors,
                    Meta = null,
                };
                // Trả về client
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 400;
                var jsonResponse = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                await context.Response.WriteAsync(jsonResponse);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<object>
                {
                    Code = HttpStatusCode.InternalServerError,
                    SystemMessage = ex.Message,
                    UserMessage = "Có lỗi xảy ra, vui lòng liên hệ MISA để được trợ giúp",
                    Data = ex.Data,
                    Meta = null,
                };
                // Trả về client
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 500;
                var jsonResponse = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                await context.Response.WriteAsync(jsonResponse);
            }
        }
    }
}
