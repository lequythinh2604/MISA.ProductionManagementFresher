using MISA.Core.Dtos;
using MISA.Core.Entities;

namespace MISA.Core.Interfaces.Service
{
    public interface IWorkShiftService : IBaseService<WorkShift>
    {
        /// <summary>
        /// Tạo mới ca làm việc
        /// </summary>
        /// <param name="request">Đối tượng payload</param>
        /// <returns>Ca làm việc vừa tạo</returns>
        /// Created by: ThinhLQ (05/12/2025)
        Task<WorkShift> CreateWorkShiftAsync(WorkShiftCreateDto request);

        /// <summary>
        /// Lấy danh sách ca làm việc theo filter, phân trang và sắp xếp
        /// </summary>
        /// <param name="request">Đối tượng chứa các tham số lọc, phân trang và sắp xếp</param>
        /// <returns>Danh sách ca làm việc theo page</returns>
        /// Created by: LQThinh (04/12/2025)
        Task<PagedResult<WorkShift>> GetPagingAsync(WorkShiftFilterDto request);

        /// <summary>
        /// Cập nhật thông tin ca làm việc 
        /// </summary>
        /// <param name="id">ID của ca làm việc cần cập nhật</param>
        /// <param name="request">Đối tượng payload</param>
        /// <returns></returns>
        /// Created by: LQThinh (05/12/2025)
        Task<WorkShift> UpdateWorkShiftAsync(string id, WorkShiftUpdateDto request);

        /// <summary>
        /// Cập nhật trạng thái ca
        /// </summary>
        /// <param name="workShiftIds"></param>
        /// <param name="newStatus"></param>
        /// <returns>Số lượng ca cập nhật</returns>
        /// Created by: LQThinh (10/12/2025)
        Task<int> UpdateMultipleStatusAsync(StatusUpdateDto request);
    }
}
