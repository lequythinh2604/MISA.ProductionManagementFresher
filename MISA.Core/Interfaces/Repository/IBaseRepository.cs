using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.Interfaces.Repository
{
    /// <summary>
    /// Interface cơ sở định nghĩa các phương thức truy xuất dữ liệu chung
    /// </summary>
    /// <typeparam name="T">Kiểu entity</typeparam>
    /// Created by: ThinhLQ (10/12/2025)
    public interface IBaseRepository<T> where T : class
    {
        /// <summary>
        /// Lấy tất cả bản ghi với khả năng tìm kiếm theo từ khóa
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm (có thể null)</param>
        /// <returns>Danh sách các bản ghi</returns>
        /// Created by: ThinhLQ (10/12/2025)
        Task<IEnumerable<T>> GetAllAsync(string? keyword);

        /// <summary>
        /// Lấy bản ghi theo ID
        /// </summary>
        /// <param name="id">ID của bản ghi</param>
        /// <returns>Bản ghi tìm được hoặc null nếu không tồn tại</returns>
        /// Created by: ThinhLQ (10/12/2025)
        Task<T?> GetByIdAsync(string id);

        /// <summary>
        /// Tạo mới một bản ghi
        /// </summary>
        /// <param name="entity">Đối tượng cần tạo</param>
        /// <returns>Đối tượng đã được tạo</returns>
        /// Created by: ThinhLQ (10/12/2025)
        Task<T> CreateAsync(T entity);

        /// <summary>
        /// Cập nhật thông tin bản ghi
        /// </summary>
        /// <param name="id">ID của bản ghi cần cập nhật</param>
        /// <param name="entity">Đối tượng chứa thông tin mới</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        /// Created by: ThinhLQ (10/12/2025)
        Task<int> UpdateAsync(string id, T entity);

        /// <summary>
        /// Xóa bản ghi theo ID
        /// </summary>
        /// <param name="id">ID của bản ghi cần xóa</param>
        /// <returns>Số bản ghi bị xóa</returns>
        /// Created by: ThinhLQ (10/12/2025)
        Task<int> DeleteAsync(string id);

        /// <summary>
        /// Kiểm tra trùng lặp giá trị của cột
        /// </summary>
        /// <param name="columnName">Tên cột cần kiểm tra</param>
        /// <param name="value">Giá trị cần kiểm tra</param>
        /// <param name="excludeId">ID cần loại trừ khi kiểm tra (dùng khi update)</param>
        /// <returns>True nếu giá trị đã tồn tại, False nếu chưa tồn tại</returns>
        /// Created by: ThinhLQ (10/12/2025)
        Task<bool> CheckDuplicateAsync(string columnName, object value, string? excludeId = null);

        /// <summary>
        /// Xóa nhiều bản ghi theo danh sách ID
        /// </summary>
        /// <param name="ids">Danh sách ID cần xóa</param>
        /// <returns>Số bản ghi bị xóa</returns>
        /// Created by: ThinhLQ (10/12/2025)
        Task<int> DeleteMultipleAsync(List<string> ids);
    }
}