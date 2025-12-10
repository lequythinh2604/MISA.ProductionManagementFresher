using MISA.Core.Dtos;
using MISA.Core.Entities;
using MISA.Core.Exceptions;
using MISA.Core.Interfaces.Repository;
using MISA.Core.Interfaces.Service;
using System.Collections;

namespace MISA.Core.Services
{
    public class WorkShiftService : IWorkShiftService
    {
        private readonly IWorkShiftRepository _workShiftRepository;
        public WorkShiftService(IWorkShiftRepository workShiftRepository)
        {
            _workShiftRepository = workShiftRepository;
        }

        /// <summary>
        /// Thêm mới ca làm việc
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Ca làm việc mới vừa thêm</returns>
        /// <exception cref="MISAValidateException"></exception>
        /// Created by: ThinhLQ (05/12/2025)
        public async Task<WorkShift> AddNewWorkShiftAsync(WorkShiftAddRequest request)
        {
            // validate
            IDictionary errors = new Dictionary<string, string>();

            // Mã ca không được để trống
            if (string.IsNullOrEmpty(request.WorkShiftCode))
            {
                errors.Add("WorkShiftCode", "Mã ca không được để trống");
            }
            // Mã ca tối đa 20 ký tự
            else if (request.WorkShiftCode.Length > 20)
            {
                errors.Add("WorkShiftCode", "Mã ca tối đa 20 ký tự");
            }
            // Mã ca không được trùng
            else if (_workShiftRepository.IsWorkShiftExists(request.WorkShiftCode))
            {
                errors.Add("WorkShiftCode", "Mã ca không được trùng");
            }

            // Tên ca không được để trống
            if (string.IsNullOrEmpty(request.WorkShiftName))
            {
                errors.Add("WorkShiftName", "Tên ca không được để trống");
            }
            // Tên ca tối đa 50 kí tự
            else if (request.WorkShiftName.Length > 50)
            {
                errors.Add("WorkShiftName", "Tên ca tối đa 50 ký tự");
            }

            // Giờ vào ca không được để trống
            if (request.StartTime == null)
            {
                errors.Add("StartTime", "Giờ vào ca không được để trống");
            }
            // Giờ hết ca không được để trống
            if (request.EndTime == null)
            {
                errors.Add("EndTime", "Giờ hết ca không được để trống");
            }

            // Giờ nghỉ phải nằm trong khoảng giờ làm
            if (request.BreakStart < request.StartTime || request.BreakStart > request.EndTime)
            {
                errors.Add("BreakStart", "Thời gian bắt đầu nghỉ giữa ca phải nằm trong khoảng thời gian tính từ giờ vào ca đến giờ hết ca");
            }
            if (request.BreakEnd > request.EndTime || request.BreakEnd < request.StartTime)
            {
                errors.Add("BreakEnd", "Thời gian kết thúc nghỉ giữa ca phải nằm trong khoảng thời gian tính từ giờ vào ca đến giờ hết ca");
            }

            if (errors.Count > 0)
            {
                throw new MISAValidateException(errors);
            }

            var result = await _workShiftRepository.AddNewWorkShiftAsync(request);
            return result;
        }

        /// <summary>
        /// Lấy danh sách ca làm việc
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Danh sách ca làm việc</returns>
        /// Created by: ThinhLQ (04/12/2025)
        public async Task<PagedResult<WorkShift>> GetPagedWorkShiftsAsync(WorkShiftFilterRequest request)
        {
            var results = await _workShiftRepository.GetPagedWorkShiftsAsync(request);
            return results;
        }

        /// <summary>
        /// Lấy ca làm việc theo Id
        /// </summary>
        /// <param name="workShiftId">Id ca làm việc</param>
        /// <returns>Ca làm việc</returns>
        /// Created by: LQThinh (05/12/2025)
        public async Task<WorkShift> GetWorkShiftByIdAsync(string workShiftId)
        {
            var result = await _workShiftRepository.GetWorkShiftByIdAsync(workShiftId);
            return result;
        }

        /// <summary>
        /// Sửa ca làm việc
        /// </summary>
        /// <param name="request"></param>
        /// <param name="workShiftId"></param>
        /// <returns>Ca làm việc vừa sửa</returns>
        /// <exception cref="MISAValidateException"></exception>
        /// Created by: LQThinh (05/12/2025)
        public async Task<WorkShift> UpdateWorkShiftAsync(string workShiftId, WorkShiftUpdateRequest request)
        {
            // validate
            IDictionary errors = new Dictionary<string, string>();

            var workShiftInDb = await _workShiftRepository.GetWorkShiftByIdAsync(workShiftId);
            if (workShiftInDb == null)
            {
                errors.Add("WorkShiftInDb", "Không tìm thấy ca làm việc");
            }

            // Mã ca không được để trống
            if (string.IsNullOrEmpty(request.WorkShiftCode))
            {
                errors.Add("WorkShiftCode", "Mã ca không được để trống");
            }
            // Mã ca tối đa 20 ký tự
            else if (request.WorkShiftCode.Length > 20)
            {
                errors.Add("WorkShiftCode", "Mã ca tối đa 20 ký tự");
            }
            // Mã ca không được trùng
            else if (_workShiftRepository.IsWorkShiftExists(request.WorkShiftCode))
            {
                errors.Add("WorkShiftCode", $"Ca làm việc <{request.WorkShiftCode}> đã tồn tại. Vui lòng kiểm tra lại.");
            }

            // Tên ca không được để trống
            if (string.IsNullOrEmpty(request.WorkShiftName))
            {
                errors.Add("WorkShiftName", "Tên ca không được để trống");
            }
            // Tên ca tối đa 50 kí tự
            else if (request.WorkShiftName.Length > 50)
            {
                errors.Add("WorkShiftName", "Tên ca tối đa 50 ký tự");
            }

            // Giờ vào ca không được để trống
            if (request.StartTime == default(DateTime))
            {
                errors.Add("StartTime", "Giờ vào ca không được để trống");
            }
            // Giờ hết ca không được để trống
            if (request.EndTime == default(DateTime))
            {
                errors.Add("EndTime", "Giờ hết ca không được để trống");
            }
            // Giờ nghỉ phải nằm trong khoảng giờ làm
            if (request.BreakStart < request.StartTime)
            {
                errors.Add("BreakStart", "Thời gian bắt đầu nghỉ giữa ca phải nằm trong khoảng thời gian tính từ giờ vào ca đến giờ hết ca");
            }
            if (request.BreakEnd > request.EndTime)
            {
                errors.Add("BreakEnd", "Thời gian kết thúc nghỉ giữa ca phải nằm trong khoảng thời gian tính từ giờ vào ca đến giờ hết ca");
            }

            if (errors.Count > 0)
            {
                throw new MISAValidateException(errors);
            }

            var updated = await _workShiftRepository.UpdateWorkShiftAsync(workShiftId, request);
            return updated;
        }

        /// <summary>
        /// Xóa ca làm việc
        /// </summary>
        /// <param name="workShiftId"></param>
        /// <returns></returns>
        /// Created by: LQThinh (05/12/2025)
        public async Task<int> DeleteWorkShiftAsync(WorkShiftDeleteDto request)
        {
            IDictionary errors = new Dictionary<string, string>();
            if (request.workShiftIds.Count == 0)
            {
                errors.Add("workShiftIds", "Không tìm thấy Ids");
            }
            if (errors.Count > 0)
            {
                throw new MISAValidateException(errors);
            }
            int affected = await _workShiftRepository.DeleteWorkShiftAsync(request);
            return affected;
        }

        /// <summary>
        /// Cập nhật trạng thái ca làm việc
        /// </summary>
        /// <param name="workShiftIds"></param>
        /// <param name="newStatus"></param>
        /// <returns></returns>
        /// <exception cref="MISAValidateException"></exception>
        /// Created by: LQThinh (10/12/2025)
        public async Task<int> UpdateMultipleStatusAsync(UpdateStatusDto request)
        {
            IDictionary errors = new Dictionary<string, string>();
            if (request.workShiftIds.Count == 0)
            {
                errors.Add("workShiftIds", "Không tìm thấy Ids");
            }
            if (errors.Count > 0)
            {
                throw new MISAValidateException(errors);
            }
            int affectedRows = await _workShiftRepository.UpdateMultipleStatusAsync(request);
            return affectedRows;
        }
    }
}
