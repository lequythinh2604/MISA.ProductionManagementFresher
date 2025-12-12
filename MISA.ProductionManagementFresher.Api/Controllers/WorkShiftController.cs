using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MISA.Core.Dtos;
using MISA.Core.Entities;
using MISA.Core.Enums;
using MISA.Core.Interfaces.Repository;
using MISA.Core.Interfaces.Service;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MISA.ProductionManagementFresher.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class WorkShiftController(IWorkShiftService workShiftService) : ControllerBase
    {
        private readonly IWorkShiftService _workShiftService = workShiftService;

        /// <summary>
        /// Lấy danh sách ca làm việc có hỗ trợ phân trang, tìm kiếm, lọc cột, sắp xếp
        /// </summary>
        /// <param name="request">Đối tượng payload</param>
        /// <returns>Danh sách ca làm việc có hỗ trợ phân trang, tìm kiếm, lọc cột, sắp xếp</returns>
        /// Created by: LQThinh (04/12/2025)
        [HttpPost("paging")]
        public async Task<IActionResult> GetPaging([FromBody] WorkShiftFilterDto request)
        {
            var pagedResult = await _workShiftService.GetPagingAsync(request);

            var response = ApiResponse<PagedResult<WorkShift>>.SuccessResponse(
                userMessage: "Lấy danh sách ca làm việc thành công",
                code: HttpStatusCode.OK,
                data: pagedResult
            );
            return Ok(response);
        }

        /// <summary>
        /// Tạo mới ca làm việc
        /// </summary>
        /// <param name="request">Đối tượng payload</param>
        /// <returns>Ca làm việc vừa tạo</returns>
        /// Created by: ThinhLQ (05/12/2025)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] WorkShiftCreateDto request)
        {
            var createdWorkShift = await _workShiftService.CreateWorkShiftAsync(request);

            var response = ApiResponse<WorkShift>.SuccessResponse(
                userMessage: "Thêm mới ca làm việc thành công",
                code: HttpStatusCode.OK,
                data: createdWorkShift
            );
            return Ok(response);
        }

        /// <summary>
        /// Lấy ca làm việc theo Id
        /// </summary>
        /// <param name="id">Id ca làm việc</param>
        /// <returns>Ca làm việc</returns>
        /// Created by: LQThinh (05/12/2025)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var entity = await _workShiftService.GetByIdAsync(id);
            var response = new ApiResponse<WorkShift>{
                Code = HttpStatusCode.OK,
                Success = entity != null,
                Data = entity,
                UserMessage = entity != null
                ? "Lấy thông tin ca thành công"
                : "Không tìm thấy ca"
            };
            return Ok(response);
        }

        /// <summary>
        /// Cập nhật thông tin ca làm việc 
        /// </summary>
        /// <param name="id">ID của ca làm việc cần cập nhật</param>
        /// <param name="request">Đối tượng payload</param>
        /// <returns></returns>
        /// Created by: LQThinh (05/12/2025)
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] WorkShiftUpdateDto request)
        {
            var updatedEntity = await _workShiftService.UpdateWorkShiftAsync(id, request);

            var response = ApiResponse<WorkShift>.SuccessResponse(
                userMessage: "Cập nhật ca làm việc thành công",
                code: HttpStatusCode.OK,
                data: updatedEntity
            );

            return Ok(response);
        }

        /// <summary>
        /// Xóa ca làm việc
        /// </summary>
        /// <param name="ids">Danh sách id cần xóa</param>
        /// <returns></returns>
        /// Created by: LQThinh (05/12/2025)
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteMultiple([FromBody] List<string> ids)
        {
            int affectedRows = await _workShiftService.DeleteMultipleAsync(ids);

            var response = ApiResponse<int>.SuccessResponse(
                userMessage: "Xóa ca làm việc thành công",
                code: HttpStatusCode.OK,
                data: affectedRows
            );

            return Ok(response);
        }

        [HttpPost("update_status")]
        public async Task<IActionResult> UpdateMultipleStatus([FromBody] StatusUpdateDto request)
        {
            int affectedRows = await _workShiftService.UpdateMultipleStatusAsync(request);

            var response = ApiResponse<int>.SuccessResponse(
                userMessage: "Cập nhật trạng thái ca làm việc thành công",
                code: HttpStatusCode.OK,
                data: affectedRows
            );

            return Ok(response);
        }
    }
}
