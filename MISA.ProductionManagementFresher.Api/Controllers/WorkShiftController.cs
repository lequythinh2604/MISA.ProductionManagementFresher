using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MISA.Core.Dtos;
using MISA.Core.Entities;
using MISA.Core.Enums;
using MISA.Core.Interfaces.Repository;
using MISA.Core.Interfaces.Service;

namespace MISA.ProductionManagementFresher.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class WorkShiftController : ControllerBase
    {
        private readonly IWorkShiftService _workShiftService;
        public WorkShiftController(IWorkShiftService workShiftService)
        {
            _workShiftService = workShiftService;
        }

        /// <summary>
        /// Lấy danh sách ca làm việc có hỗ trợ phân trang, tìm kiếm, lọc cột, sắp xếp
        /// </summary>
        /// <param name="workShiftFilterRequest"></param>
        /// <returns>Danh sách ca làm việc</returns>
        /// /// Created by: LQThinh (04/12/2025)
        [HttpPost("paging")]
        public async Task<IActionResult> GetPaging([FromBody] WorkShiftFilterRequest workShiftFilterRequest)
        {
            var request = new WorkShiftFilterRequest
            {
                Page = workShiftFilterRequest.Page,
                PageSize = workShiftFilterRequest.PageSize,
                Keyword = workShiftFilterRequest.Keyword,
                SortColumn = workShiftFilterRequest.SortColumn,
                FilterColumn = workShiftFilterRequest.FilterColumn,
            };
            
            var results = await _workShiftService.GetPagedWorkShiftsAsync(request);
            var response = new ApiResponse<IEnumerable<WorkShift>>
            {
                Code = HttpStatusCode.OK,
                Data = results.Items,
                Meta = new Meta
                {
                    Page = results.Page,
                    PageSize= results.PageSize,
                    TotalCount = results.TotalCount,
                }
            };

            return Ok(response);
        }

        /// <summary>
        /// Thêm mới ca làm việc
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Ca làm việc mới vừa thêm</returns>
        /// Created by: ThinhLQ (05/12/2025)
        [HttpPost]
        public async Task<IActionResult> AddNewWorkShift([FromBody] WorkShiftAddRequest request)
        {
            var result = await _workShiftService.AddNewWorkShiftAsync(request);
            var response = new ApiResponse<WorkShift>
            {
                Code = HttpStatusCode.Created,
                Data = result,
                SystemMessage = "Thêm mới ca làm việc thành công",
                Meta = null
            };
            return Ok(response);
        }

        /// <summary>
        /// Lấy ca làm việc theo Id
        /// </summary>
        /// <param name="workShiftId">Id ca làm việc</param>
        /// <returns>Ca làm việc</returns>
        /// Created by: LQThinh (05/12/2025)
        [HttpGet("{workShiftId}")]
        public async Task<IActionResult> GetWorkShiftById(string workShiftId)
        {
            var result = await _workShiftService.GetWorkShiftByIdAsync(workShiftId);
            if (result == null)
            {
                return NotFound("Không tìm thấy ca làm việc");
            }
            var response = new ApiResponse<WorkShift>
            {
                Code = HttpStatusCode.OK,
                Data = result,
                SystemMessage = null,
                Meta = null
            };
            return Ok(response);
        }

        /// <summary>
        /// Sửa ca làm việc
        /// </summary>
        /// <param name="workShiftId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// Created by: LQThinh (05/12/2025)
        [HttpPut("{workShiftId}")]
        public async Task<IActionResult> UpdateWorkShift([FromRoute] string workShiftId, [FromBody] WorkShiftUpdateRequest request)
        {
            var updated = await _workShiftService.UpdateWorkShiftAsync(workShiftId, request);
            var response = new ApiResponse<WorkShift>
            {
                Code = HttpStatusCode.OK,
                Data = updated,
                SystemMessage = "Sửa ca làm việc thành công",
                Meta = null
            };
            return Ok(response);
        }

        /// <summary>
        /// Xóa ca làm việc
        /// </summary>
        /// <param name="workShiftId"></param>
        /// <returns></returns>
        /// Created by: LQThinh (05/12/2025)
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteWorkShift([FromBody] WorkShiftDeleteDto request)
        {
            int affectedRows =  await _workShiftService.DeleteWorkShiftAsync(request);
            var response = new ApiResponse<object>
            {
                Code = HttpStatusCode.OK,
                Data = affectedRows,
                SystemMessage = "Xóa ca làm việc thành công",
                Meta = null
            };
            return Ok(response);
        }

        [HttpPost("update_status")]
        public async Task<IActionResult> UpdateMultipleStatus([FromBody] UpdateStatusDto request)
        {
            int affectedRows = await _workShiftService.UpdateMultipleStatusAsync(request);
            var response = new ApiResponse<object>
            {
                Code = HttpStatusCode.OK,
                Data = affectedRows,
                SystemMessage = "Cập nhật trạng thái ca thành công",
                Meta = null
            };
            return Ok(response);
        }
    }
}
