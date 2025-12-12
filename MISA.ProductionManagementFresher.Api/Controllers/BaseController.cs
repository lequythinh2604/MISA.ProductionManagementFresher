using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.Core.Interfaces.Service;

namespace MISA.ProductionManagementFresher.Api.Controllers
{
    /// <summary>
    /// Controller cơ sở cung cấp các API CRUD chung
    /// </summary>
    /// <typeparam name="T">Kiểu entity</typeparam>
    /// Created by: LQThinh (04/12/2025)
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseController<T>(IBaseService<T> baseService) : ControllerBase where T : class
    {
        /// <summary>
        /// Lấy tất cả bản ghi với khả năng tìm kiếm
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm</param>
        /// <returns>Danh sách các bản ghi</returns>
        /// Created by: LQThinh (04/12/2025)
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? keyword = null)
        {
            var entities = await baseService.GetAllAsync(keyword);
            return Ok(entities);
        }

        /// <summary>
        /// Lấy bản ghi theo ID
        /// </summary>
        /// <param name="id">ID của bản ghi</param>
        /// <returns>Thông tin bản ghi</returns>
        /// Created by: LQThinh (04/12/2025)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var entity = await baseService.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }
            return Ok(entity);
        }

        /// <summary>
        /// Tạo mới một bản ghi
        /// </summary>
        /// <param name="entity">Thông tin bản ghi cần tạo</param>
        /// <returns>Bản ghi đã được tạo</returns>
        /// Created by: LQThinh (04/12/2025)
        [HttpPost]
        public async Task<IActionResult> Create(T entity)
        {
            var createdEntity = await baseService.CreateAsync(entity);
            return Ok(createdEntity);
        }

        /// <summary>
        /// Cập nhật thông tin bản ghi
        /// </summary>
        /// <param name="id">ID của bản ghi cần cập nhật</param>
        /// <param name="entity">Thông tin bản ghi mới</param>
        /// <returns>Kết quả cập nhật</returns>
        /// Created by: LQThinh (04/12/2025)
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, T entity)
        {
            var updatedEntity = await baseService.UpdateAsync(id, entity);
            return Ok(updatedEntity);
        }

        /// <summary>
        /// Xóa bản ghi theo ID
        /// </summary>
        /// <param name="id">ID của bản ghi cần xóa</param>
        /// <returns>Kết quả xóa</returns>
        /// Created by: LQThinh (04/12/2025)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await baseService.DeleteAsync(id);
            return NoContent();
        }
    }
}
