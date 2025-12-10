
using MISA.Core.Enums;

namespace MISA.Core.Dtos
{
    public class ApiResponse<T>
    {
        public HttpStatusCode? Code { get; set; }
        public T? Data { get; set; }
        public string? UserMessage { get; set; }
        public string? SystemMessage { get; set; }
        public Meta? Meta { get; set; } = new Meta();
    }
    public class Meta
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public int? TotalCount { get; set; }
    }
}
