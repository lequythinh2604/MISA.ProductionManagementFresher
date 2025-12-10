using MISA.Core.Dtos;
using MISA.Core.Entities;

namespace MISA.Core.Interfaces.Service
{
    public interface IWorkShiftService
    {
        /// <summary>
        /// Lấy danh sách ca làm việc theo filter, phân trang và sắp xếp
        /// </summary>
        /// <param name="request">Đối tượng chứa các tham số lọc, phân trang và sắp xếp</param>
        /// <returns>Danh sách ca làm việc theo page</returns>
        /// Created by: LQThinh (04/12/2025)
        Task<PagedResult<WorkShift>> GetPagedWorkShiftsAsync(WorkShiftFilterRequest request);

        /// <summary>
        /// Thêm mới ca làm việc
        /// </summary>
        /// <param name="newWorkShift">Đối tượng ca làm việc</param>
        /// <returns>Ca làm việc vừa thêm</returns>
        /// Created by: LQThinh (05/12/2025)
        Task<WorkShift> AddNewWorkShiftAsync(WorkShiftAddRequest request);

        /// <summary>
        /// Lấy ca làm việc theo Id
        /// </summary>
        /// <param name="workShiftId">Id ca làm việc</param>
        /// <returns>Ca làm việc</returns>
        /// Created by: LQThinh (05/12/2025)
        Task<WorkShift> GetWorkShiftByIdAsync(string workShiftId);

        /// <summary>
        /// Sửa ca làm việc
        /// </summary>
        /// <param name="request"></param>
        /// <param name="workShiftId"></param>
        /// <returns>Ca làm việc vừa sửa</returns>
        /// Created by: LQThinh (05/12/2025)
        Task<WorkShift> UpdateWorkShiftAsync(string workShiftId, WorkShiftUpdateRequest request);

        /// <summary>
        /// Cập nhật trạng thái ca
        /// </summary>
        /// <param name="workShiftIds"></param>
        /// <param name="newStatus"></param>
        /// <returns>Số lượng ca cập nhật</returns>
        /// Created by: LQThinh (10/12/2025)
        Task<int> UpdateMultipleStatusAsync(UpdateStatusDto request);

        /// <summary>
        /// Xóa ca làm việc
        /// </summary>
        /// <param name="workShiftId"></param>
        /// <returns></returns>
        /// Created by: LQThinh (05/12/2025)
        Task<int> DeleteWorkShiftAsync(WorkShiftDeleteDto request);
    }
}
