using MISA.Core.Exceptions;
using MISA.Core.Interfaces.Repository;
using MISA.Core.Interfaces.Service;
using MISA.Core.MISAAttributes;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MISA.Core.Services
{
    /// <summary>
    /// Service cơ sở cung cấp các chức năng CRUD chung
    /// </summary>
    /// <typeparam name="T">Kiểu entity</typeparam>
    /// Created by: ThinhLQ (10/12/2025)
    public class BaseService<T>(IBaseRepository<T> baseRepository) : IBaseService<T> where T : class
    {
        /// <summary>
        /// Tạo mới một bản ghi sau khi validate
        /// </summary>
        /// <param name="entity">Đối tượng cần tạo</param>
        /// <returns>Đối tượng đã được tạo</returns>
        /// Created by: ThinhLQ (05/12/2025)
        public async Task<T> CreateAsync(T entity)
        {
            await ValidateEntityAsync(entity);
            return await baseRepository.CreateAsync(entity);
        }

        /// <summary>
        /// Xóa bản ghi theo ID
        /// </summary>
        /// <param name="id">ID của bản ghi cần xóa</param>
        /// <returns>Số bản ghi bị xóa</returns>
        /// Created by: ThinhLQ (05/12/2025)
        public Task<int> DeleteAsync(string id)
        {
            return baseRepository.DeleteAsync(id);
        }

        /// <summary>
        /// Xóa nhiều bản ghi theo danh sách ID
        /// </summary>
        /// <param name="ids">Danh sách ID cần xóa</param>
        /// <returns>Số bản ghi bị xóa</returns>
        /// Created by: ThinhLQ (10/12/2025)
        public async Task<int> DeleteMultipleAsync(List<string> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                throw new MISAValidateException("Danh sách ID không được để trống");
            }

            return await baseRepository.DeleteMultipleAsync(ids);
        }

        /// <summary>
        /// Lấy tất cả bản ghi với khả năng tìm kiếm theo từ khóa
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm (có thể null)</param>
        /// <returns>Danh sách các bản ghi</returns>
        /// Created by: ThinhLQ (05/12/2025)
        public Task<IEnumerable<T>> GetAllAsync(string? keyword)
        {
            return baseRepository.GetAllAsync(keyword);
        }

        /// <summary>
        /// Lấy bản ghi theo ID
        /// </summary>
        /// <param name="id">ID của bản ghi</param>
        /// <returns>Bản ghi tìm được hoặc null nếu không tồn tại</returns>
        /// Created by: LQThinh (05/12/2025)
        public Task<T?> GetByIdAsync(string id)
        {
            return baseRepository.GetByIdAsync(id);
        }

        /// <summary>
        /// Cập nhật thông tin bản ghi sau khi validate
        /// </summary>
        /// <param name="id">ID của bản ghi cần cập nhật</param>
        /// <param name="entity">Đối tượng chứa thông tin mới</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        /// Created by: ThinhLQ (10/12/2025)
        public async Task<int> UpdateAsync(string id, T entity)
        {
            await ValidateEntityAsync(entity, id);
            return await baseRepository.UpdateAsync(id, entity);
        }

        /// <summary>
        /// Validate thông tin entity (kiểm tra required và duplicate)
        /// </summary>
        /// <param name="entity">Đối tượng cần validate</param>
        /// <param name="excludeId">ID cần loại trừ khi kiểm tra trùng lặp (dùng khi update)</param>
        /// <returns>True nếu hợp lệ</returns>
        /// <exception cref="ValidationException">Khi dữ liệu không hợp lệ</exception>
        /// <exception cref="DuplicateException">Khi có dữ liệu trùng lặp</exception>
        /// Created by: ThinhLQ (05/12/2025)
        public async Task<bool> ValidateEntityAsync(T entity, string? excludeId = null)
        {
            var properties = typeof(T).GetProperties();
            foreach (var prop in properties)
            {
                var requiredAttr = prop.GetCustomAttributes(typeof(MISARequired), true);
                if (requiredAttr.Length > 0)
                {
                    var value = prop.GetValue(entity);
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
                    var value = prop.GetValue(entity);
                    if (value != null)
                    {
                        var columnAttr = prop.GetCustomAttribute<MISAColumnName>();
                        var columnName = columnAttr != null ? columnAttr.ColumnName : prop.Name.ToLower();

                        var isDuplicate = await baseRepository.CheckDuplicateAsync(columnName, value, excludeId);
                        if (isDuplicate)
                        {
                            throw new MISADuplicateException("Không được phép trùng lặp giá trị.", prop.Name, value.ToString() ?? "");
                        }
                    }
                }
            }

            return true;
        }
    }
}
