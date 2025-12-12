using MISA.Core.Dtos;
using MISA.Core.Entities;
using MISA.Core.Exceptions;
using MISA.Core.Interfaces.Repository;
using MISA.Core.Interfaces.Service;
using MISA.Core.MISAAttributes;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MISA.Core.Services
{
    /// <summary>
    /// Service xử lý nghiệp vụ liên quan đến ca làm việc
    /// </summary>
    /// Created by: LQThinh (06/12/2025)
    public class WorkShiftService(IWorkShiftRepository workShiftRepository) : BaseService<WorkShift>(workShiftRepository), IWorkShiftService
    {
        private readonly IWorkShiftRepository _workShiftRepository = workShiftRepository;

        /// <summary>
        /// Lấy danh sách ca làm việc có hỗ trợ phân trang, tìm kiếm, lọc cột, sắp xếp
        /// </summary>
        /// <param name="request">Đối tượng payload</param>
        /// <returns>Danh sách ca làm việc có hỗ trợ phân trang, tìm kiếm, lọc cột, sắp xếp</returns>
        /// Created by: LQThinh (04/12/2025)
        public async Task<PagedResult<WorkShift>> GetPagingAsync(WorkShiftFilterDto request)
        {
            var pagedResult = await _workShiftRepository.GetPagingAsync(request);
            return pagedResult;
        }

        /// <summary>
        /// Tạo mới ca làm việc
        /// </summary>
        /// <param name="request">Đối tượng payload</param>
        /// <returns>Ca làm việc vừa tạo</returns>
        /// Created by: ThinhLQ (05/12/2025)
        public async Task<WorkShift> CreateWorkShiftAsync(WorkShiftCreateDto request)
        {
            await ValidateShiftCreateDtoAsync(request);

            // Giờ nghỉ phải nằm trong khoảng giờ làm
            if (request.BreakStart < request.StartTime || request.BreakStart > request.EndTime)
            {
                throw new MISAValidateException($"Thời gian bắt đầu nghỉ giữa ca phải nằm trong khoảng thời gian tính từ giờ vào ca đến giờ hết ca");
            }
            if (request.BreakEnd > request.EndTime || request.BreakEnd < request.StartTime)
            {
                throw new MISAValidateException($"Thời gian kết thúc nghỉ giữa ca phải nằm trong khoảng thời gian tính từ giờ vào ca đến giờ hết ca");
            }

            var workShift = MapToWorkShift(request);

            var createdWorkShift = await _workShiftRepository.CreateAsync(workShift);

            return createdWorkShift;
        }

        /// <summary>
        /// Cập nhật thông tin ca làm việc 
        /// </summary>
        /// <param name="id">ID của ca làm việc cần cập nhật</param>
        /// <param name="request">Đối tượng payload</param>
        /// <returns></returns>
        /// Created by: LQThinh (05/12/2025)
        public async Task<WorkShift> UpdateWorkShiftAsync(string id, WorkShiftUpdateDto request)
        {
            var workShiftInDb = await _workShiftRepository.GetByIdAsync(id);
            if (workShiftInDb == null)
            {
                throw new MISANotFoundException("Ca làm việc không tồn tại trong hệ thống.");
            }

            await ValidateShiftUpdateDtoAsync(request, workShiftInDb.WorkShiftCode);

            // Giờ nghỉ phải nằm trong khoảng giờ làm
            if (request.BreakStart < request.StartTime || request.BreakStart > request.EndTime)
            {
                throw new MISAValidateException($"Thời gian bắt đầu nghỉ giữa ca phải nằm trong khoảng thời gian tính từ giờ vào ca đến giờ hết ca");
            }
            if (request.BreakEnd > request.EndTime || request.BreakEnd < request.StartTime)
            {
                throw new MISAValidateException($"Thời gian kết thúc nghỉ giữa ca phải nằm trong khoảng thời gian tính từ giờ vào ca đến giờ hết ca");
            }

            var updatedEntity = await _workShiftRepository.UpdateWorkShiftAsync(id, request);

            return updatedEntity;
        }

        /// <summary>
        /// Validate thông tin ca làm việc từ DTO
        /// </summary>
        /// <param name="request">DTO chứa thông tin ca làm việc cần validate</param>
        /// <param name="excludeId">ID cần loại trừ khi kiểm tra trùng lặp (dùng khi update)</param>
        /// <returns>True nếu hợp lệ</returns>
        /// <exception cref="MISAValidateException"></exception>
        /// <exception cref="MISADuplicateException"></exception>
        /// Created by: ThinhLQ (05/12/2025)
        private async Task<bool> ValidateShiftCreateDtoAsync(WorkShiftCreateDto request, string? excludeId = null)
        {
            var properties = typeof(WorkShiftCreateDto).GetProperties();
            foreach (var prop in properties)
            {
                var requiredAttr = prop.GetCustomAttributes(typeof(MISARequired), true);
                if (requiredAttr.Length > 0)
                {
                    var value = prop.GetValue(request);
                    if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
                    {
                        throw new MISAValidateException($"{prop.Name} không được để trống.");
                    }
                }
            }

            foreach (var prop in properties)
            {
                var duplicateAttr = prop.GetCustomAttribute<MISACheckDuplicate>();
                if (duplicateAttr != null)
                {
                    var value = prop.GetValue(request);
                    if (value != null)
                    {
                        var columnAttr = prop.GetCustomAttribute<MISAColumnName>();
                        var columnName = columnAttr != null ? columnAttr.ColumnName : prop.Name.ToLower();

                        var isDuplicate = await _workShiftRepository.CheckDuplicateAsync(columnName, value, excludeId);
                        if (isDuplicate)
                        {
                            throw new MISADuplicateException("Không được phép trùng lặp giá trị.", prop.Name, value.ToString() ?? "");
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Chuyển đổi từ WorkShiftCreateDto sang WorkShift entity
        /// </summary>
        /// <param name="request">DTO chứa thông tin ca làm việc</param>
        /// <returns>WorkShift entity</returns>
        /// Created by: ThinhLQ (05/12/2025)
        private static WorkShift MapToWorkShift(WorkShiftCreateDto request)
        {
            var workShift = new WorkShift
            {
                WorkShiftCode = request.WorkShiftCode,
                WorkShiftName = request.WorkShiftName,
                StartTime = request.StartTime ?? DateTime.MinValue,
                EndTime = request.EndTime ?? DateTime.MinValue,
                BreakStart = request.BreakStart,
                BreakEnd = request.BreakEnd,
                WorkingHours = request.WorkingHours,
                BreakHours = request.BreakHours,
                WorkShiftStatus = request.WorkShiftStatus ?? true,
                CreatedDate = request.CreatedDate,
                CreatedBy = request.CreatedBy,
                ModifiedDate = request.ModifiedDate,
                ModifiedBy = request.ModifiedBy,
                Description = request.Description
            };
            return workShift;
        }

        /// <summary>
        /// Validate thông tin ca làm việc từ DTO
        /// </summary>
        /// <param name="request">DTO chứa thông tin ca làm việc cần validate</param>
        /// <param name="excludeId">ID cần loại trừ khi kiểm tra trùng lặp (dùng khi update)</param>
        /// <returns>True nếu hợp lệ</returns>
        /// <exception cref="MISAValidateException"></exception>
        /// <exception cref="MISADuplicateException"></exception>
        /// Created by: ThinhLQ (05/12/2025)
        private async Task<bool> ValidateShiftUpdateDtoAsync(WorkShiftUpdateDto request, string? excludeId = null)
        {
            var properties = typeof(WorkShiftUpdateDto).GetProperties();
            foreach (var prop in properties)
            {
                var requiredAttr = prop.GetCustomAttributes(typeof(MISARequired), true);
                if (requiredAttr.Length > 0)
                {
                    var value = prop.GetValue(request);
                    if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
                    {
                        throw new MISAValidateException($"{prop.Name} không được để trống.");
                    }
                }
            }

            foreach (var prop in properties)
            {
                var duplicateAttr = prop.GetCustomAttribute<MISACheckDuplicate>();
                if (duplicateAttr != null)
                {
                    var value = prop.GetValue(request);
                    if (value != null)
                    {
                        var columnAttr = prop.GetCustomAttribute<MISAColumnName>();
                        var columnName = columnAttr != null ? columnAttr.ColumnName : prop.Name.ToLower();

                        var isDuplicate = await _workShiftRepository.CheckDuplicateAsync(columnName, value, excludeId);
                        if (isDuplicate)
                        {
                            throw new MISADuplicateException("Không được phép trùng lặp giá trị.", prop.Name, value.ToString() ?? "");
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Cập nhật trạng thái ca làm việc
        /// </summary>
        /// <param name="request">Đối tượng payload</param>
        /// <returns>Số bản ghi được cập nhật trạng thái</returns>
        /// <exception cref="MISAValidateException"></exception>
        /// Created by: LQThinh (10/12/2025)
        public async Task<int> UpdateMultipleStatusAsync(StatusUpdateDto request)
        {
            if (request.ids.Count == 0)
            {
                throw new MISAValidateException("Danh sách ID không được để trống");
            }
            int affectedRows = await _workShiftRepository.UpdateMultipleStatusAsync(request);
            return affectedRows;
        }
    }
}
