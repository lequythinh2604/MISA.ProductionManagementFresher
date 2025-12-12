using Dapper;
using Microsoft.Extensions.Configuration;
using MISA.Core.Entities;
using MISA.Core.Interfaces.Repository;
using MISA.Core.MISAAttributes;
using MySqlConnector;
using System.Data;
using System.Reflection;

namespace MISA.Infrastructure.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T>, IDisposable where T : class
    {
        protected readonly string connectionString;
        protected IDbConnection dbConnection;

        /// <summary>
        /// Khởi tạo repository với cấu hình kết nối database
        /// </summary>
        /// <param name="configuration">Cấu hình ứng dụng</param>
        /// Created by: ThinhLQ (10/12/2025)
        public BaseRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
            dbConnection = new MySqlConnection(connectionString);
        }

        /// <summary>
        /// Kiểm tra trùng lặp giá trị của cột
        /// </summary>
        /// <param name="columnName">Tên cột cần kiểm tra</param>
        /// <param name="value">Giá trị cần kiểm tra</param>
        /// <param name="excludeId">ID cần loại trừ khi kiểm tra (dùng khi update)</param>
        /// <returns>True nếu giá trị đã tồn tại, False nếu chưa tồn tại</returns>
        /// Created by: ThinhLQ (10/12/2025)
        public async Task<bool> CheckDuplicateAsync(string columnName, object value, string? excludeId = null)
        {
            var tableAttr = typeof(T).GetCustomAttribute<MISATableName>();
            var tableName = tableAttr != null ? tableAttr.TableName : typeof(T).Name.ToLower();

            var sqlCommand = !string.IsNullOrEmpty(excludeId)
                ? $"SELECT COUNT(*) FROM {tableName} WHERE {columnName} = @Value AND {tableName}_code != @ExcludeId"
                : $"SELECT COUNT(*) FROM {tableName} WHERE {columnName} = @Value";

            var count = await dbConnection.ExecuteScalarAsync<int>(sqlCommand, new
            {
                Value = value,
                ExcludeId = excludeId
            });

            return count > 0;
        }

        /// <summary>
        /// Tạo mới một bản ghi
        /// </summary>
        /// <param name="entity">Đối tượng cần tạo</param>
        /// <returns>Đối tượng đã được tạo</returns>
        /// Created by: ThinhLQ (10/12/2025)
        public async Task<T> CreateAsync(T entity)
        {
            var properties = typeof(T).GetProperties();
            var tableAttr = typeof(T).GetCustomAttribute<MISATableName>();
            var tableName = tableAttr != null ? tableAttr.TableName : typeof(T).Name.ToLower();
            var columns = "";
            var columnParams = "";
            var parameters = new DynamicParameters();
            foreach (var prop in properties)
            {
                var columnAttr = prop.GetCustomAttribute<MISAColumnName>();
                var columnName = columnAttr != null ? columnAttr.ColumnName : prop.Name.ToLower();

                columns += $"{columnName}, ";
                columnParams += $"@{prop.Name}, ";
                parameters.Add($"@{prop.Name}", prop.GetValue(entity));
            }
            columns = columns.TrimEnd(',', ' ');
            columnParams = columnParams.TrimEnd(',', ' ');

            var sqlCommand = $"INSERT INTO {tableName} ({columns}) VALUES ({columnParams})";

            await dbConnection.ExecuteAsync(sqlCommand, parameters);

            return entity;
        }

        /// <summary>
        /// Xóa bản ghi theo ID
        /// </summary>
        /// <param name="id">ID của bản ghi cần xóa</param>
        /// <returns>Số bản ghi bị xóa</returns>
        /// Created by: ThinhLQ (10/12/2025)
        public async Task<int> DeleteAsync(string id)
        {
            var tableAttr = typeof(T).GetCustomAttribute<MISATableName>();
            var tableName = tableAttr != null ? tableAttr.TableName : typeof(T).Name.ToLower();

            var sqlCommand = $"DELETE FROM {tableName} WHERE {tableName}_id = @Id";

            var result = await dbConnection.ExecuteAsync(sqlCommand, new { Id = id });

            return result;
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
                return 0;
            }

            var tableAttr = typeof(T).GetCustomAttribute<MISATableName>();
            var tableName = tableAttr != null ? tableAttr.TableName : typeof(T).Name.ToLower();

            var parameters = new DynamicParameters();
            var paramNames = new List<string>();

            for (int i = 0; i < ids.Count; i++)
            {
                var paramName = $"@Id{i}";
                paramNames.Add(paramName);
                parameters.Add(paramName, ids[i]);
            }

            var sqlCommand = $"DELETE FROM {tableName} WHERE {tableName}_id IN ({string.Join(",", paramNames)})";

            var result = await dbConnection.ExecuteAsync(sqlCommand, parameters);

            return result;
        }

        /// <summary>
        /// Giải phóng tài nguyên kết nối database
        /// </summary>
        /// Created by: LQThinh (05/12/2025)
        public void Dispose()
        {
            dbConnection?.Dispose();
        }

        /// <summary>
        /// Lấy tất cả bản ghi với khả năng tìm kiếm theo từ khóa
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm (có thể null)</param>
        /// <returns>Danh sách các bản ghi</returns>
        /// Created by: ThinhLQ (05/12/2025)
        public async Task<IEnumerable<T>> GetAllAsync(string? keyword)
        {
            var tableAttr = typeof(T).GetCustomAttribute<MISATableName>();
            var tableName = tableAttr != null ? tableAttr.TableName : typeof(T).Name.ToLower();

            var parameters = new DynamicParameters();

            var whereClauses = new List<string>();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                whereClauses.Add("(asset_code LIKE @Keyword OR asset_name LIKE @Keyword)");
                parameters.Add("@Keyword", $"%{keyword}%");
            }

            var whereClause = whereClauses.Count > 0 ? " WHERE " + string.Join(" AND ", whereClauses) : "";

            var sqlCommand = $@"SELECT * FROM {tableName}{whereClause}";

            var data = await dbConnection.QueryAsync<T>(sqlCommand, parameters);

            return data;
        }

        /// <summary>
        /// Lấy bản ghi theo ID
        /// </summary>
        /// <param name="id">ID của bản ghi</param>
        /// <returns>Bản ghi tìm được hoặc null nếu không tồn tại</returns>
        /// Created by: LQThinh (05/12/2025)
        public async Task<T?> GetByIdAsync(string id)
        {
            var tableAttr = typeof(T).GetCustomAttribute<MISATableName>();
            var tableName = tableAttr != null ? tableAttr.TableName : typeof(T).Name.ToLower();

            var sqlCommand = $"SELECT * FROM {tableName} WHERE {tableName}_id = @Id";

            var result = await dbConnection.QueryFirstOrDefaultAsync<T>(sqlCommand, new { Id = id });

            return result;
        }

        /// <summary>
        /// Cập nhật thông tin bản ghi
        /// </summary>
        /// <param name="id">ID của bản ghi cần cập nhật</param>
        /// <param name="entity">Đối tượng chứa thông tin mới</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        /// Created by: LQThinh (05/12/2025)
        public Task<int> UpdateAsync(string id, T entity)
        {
            var properties = typeof(T).GetProperties();
            var tableAttr = typeof(T).GetCustomAttribute<MISATableName>();
            var tableName = tableAttr != null ? tableAttr.TableName : typeof(T).Name.ToLower();
            var setClause = "";
            var parameters = new DynamicParameters();
            foreach (var prop in properties)
            {
                if (prop.GetCustomAttribute<MISAKey>() != null)
                {
                    continue;
                }
                var columnAttr = prop.GetCustomAttribute<MISAColumnName>();
                var columnName = columnAttr != null ? columnAttr.ColumnName : prop.Name;
                setClause += $"{columnName} = @{prop.Name},";
                parameters.Add($"@{prop.Name}", prop.GetValue(entity));
            }
            setClause = setClause.TrimEnd(',');
            var sqlCommand = $"UPDATE {tableName} SET {setClause} WHERE {tableName}_id = @Id";
            parameters.Add("@Id", id);
            var result = dbConnection.ExecuteAsync(sqlCommand, parameters);

            return result;
        }
    }
}
