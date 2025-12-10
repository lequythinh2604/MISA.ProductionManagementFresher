using Dapper;
using MISA.Core.Dtos;
using MISA.Core.Entities;
using MISA.Core.Interfaces.Repository;
using MISA.Core.Utilities;
using MySqlConnector;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Text;

namespace MISA.Infrastructure.Repositories
{
    public class WorkShiftRepository : IWorkShiftRepository
    {
        private readonly string _connectionString;
        public WorkShiftRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Thêm mới ca làm việc
        /// </summary>
        /// <param name="newWorkShift">Đối tượng ca làm việc</param>
        /// <returns>Ca làm việc vừa thêm</returns>
        /// /// Created by: LQThinh (04/12/2025)
        public async Task<WorkShift> AddNewWorkShiftAsync(WorkShiftAddRequest request)
        {
            var newId = Guid.NewGuid();
            var sql = @"
                INSERT INTO work_shift (
                    work_shift_id,
                    work_shift_code,
                    work_shift_name,
                    start_time,
                    end_time,
                    break_start,
                    break_end,
                    working_hours,
                    break_hours,
                    work_shift_status,
                    created_by,
                    created_date,
                    modified_by,
                    modified_date,
                    description
                ) VALUES (
                    @WorkShiftId,
                    @WorkShiftCode,
                    @WorkShiftName,
                    @StartTime,
                    @EndTime,
                    @BreakStart,
                    @BreakEnd,
                    @WorkingHours,
                    @BreakHours,
                    @WorkShiftStatus,
                    @CreatedBy,
                    NOW(),
                    @ModifiedBy,
                    NOW(),
                    @Description
                );";

            var parameters = new
            {
                WorkShiftId = newId,
                WorkShiftCode = request.WorkShiftCode,
                WorkShiftName = request.WorkShiftName,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                BreakStart = request.BreakStart,
                BreakEnd = request.BreakEnd,
                WorkingHours = request.WorkingHours,
                BreakHours = request.BreakHours,
                WorkShiftStatus = request.WorkShiftStatus,
                CreatedBy = "ThinhLQ",
                ModifiedBy = "ThinhLQ",
                Description = request.Description,
            };

            using (var db = new MySqlConnection(_connectionString))
            {               
                await db.ExecuteAsync(sql, parameters);

                var selectSql = @"
                    SELECT *
                    FROM work_shift
                    WHERE work_shift_id = @Id;";
                var inserted = await db.QuerySingleAsync<WorkShift>(selectSql, new { Id = newId });
                return inserted;
            }
        }

        /// <summary>
        /// Lấy danh sách ca làm việc theo filter, phân trang và sắp xếp
        /// </summary>
        /// <param name="request">Đối tượng chứa các tham số lọc, phân trang và sắp xếp</param>
        /// <returns>Danh sách ca làm việc theo page</returns>
        /// Created by: LQThinh (04/12/2025)
        public async Task<PagedResult<WorkShift>> GetPagedWorkShiftsAsync(WorkShiftFilterRequest request)
        {
                // quy định đầu vào phân trang
                var page = request.Page <= 0 ? 1 : request.Page;
                var pageSize = request.PageSize <= 0 ? 20 : request.PageSize;
                var offset = (page - 1) * pageSize;

                // khởi tạo câu lệnh where và param
                var whereClauses = new List<string>();
                var sortClauses = new List<string>();
                var dynamicParameters = new DynamicParameters();

                // tìm kiếm theo keyword
                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    var searchColumns = new List<string> { 
                        "work_shift_code",
                        "work_shift_name",
                        "created_by",
                        "modified_by"
                    };
                    var searchByConditions = searchColumns
                        .Select(col => $"{col} LIKE @keyword")
                        .ToArray();
                    whereClauses.Add($"({string.Join(" OR ", searchByConditions)}) ");
                    dynamicParameters.Add("@keyword", $"%{request.Keyword}%");
                };

                // Tìm kiếm theo cột
                if (request.FilterColumn.Count > 0)
                {
                    var paramIndex = 0;
                    foreach ( var filter in request.FilterColumn )
                    {
                        if (filter == null || string.IsNullOrWhiteSpace(filter.ColumnName)) continue;
                        var paramName = $"@f{paramIndex}";

                        switch (filter.FilterOperator)
                        {
                            case "contains":
                                whereClauses.Add($"{filter.ColumnName} LIKE {paramName}");
                                dynamicParameters.Add(paramName, $"%{filter.FilterValue}%");
                                break;
                            case "not contains":
                                whereClauses.Add($"{filter.ColumnName} NOT LIKE {paramName}");
                                dynamicParameters.Add(paramName, $"%{filter.FilterValue}%");
                                break;
                            case "=":
                                whereClauses.Add($"{filter.ColumnName} = {paramName}");
                                //dynamicParameters.Add(paramName, b);
                                break;
                            case null:
                                break;
                            default:
                                break;
                        }
                        paramIndex++;
                    }
                }

                // Sắp xếp theo cột
                if (request.SortColumn.Count > 0)
                {
                    foreach ( var sort in request.SortColumn )
                    {
                        if (string.IsNullOrWhiteSpace(sort?.Selector)) continue;

                        var direction = sort.Desc ? "DESC" : "ASC";
                        sortClauses.Add($"{sort.Selector} {direction}");
                    }
                }

                var whereClause = whereClauses.Any() ? " WHERE " + string.Join(" AND ", whereClauses) : string.Empty;
                var sortClause = sortClauses.Any() ? " ORDER BY " + string.Join(", ", sortClauses) : "";

                // dataSql
                var dataSql = $@"
                                SELECT
                                  work_shift_id,
                                  work_shift_code,
                                  work_shift_name,
                                  start_time,
                                  end_time,
                                  break_start,
                                  break_end,
                                  working_hours,
                                  break_hours,
                                  work_shift_status,
                                  created_by,
                                  created_date,
                                  modified_by,
                                  modified_date,
                                  description
                                FROM work_shift
                                {whereClause}
                                {sortClause}
                                LIMIT @limit OFFSET @offset;";
                dynamicParameters.Add("@limit", pageSize);
                dynamicParameters.Add("@offset", offset);
                
                // kết nối db
                using (var db = new MySqlConnection(_connectionString))
                {
                    var items = (await db.QueryAsync<WorkShift>(dataSql, dynamicParameters)).ToList();
                    var countSql = $"SELECT COUNT(*) FROM work_shift {whereClause};";
                    var total = await db.ExecuteScalarAsync<int>(countSql, dynamicParameters);

                    var pagedResult = new PagedResult<WorkShift>
                    {
                        Items = items,
                        Page = page,
                        PageSize = pageSize,
                        TotalCount = total,
                    };

                    return pagedResult;
                }
        }

        /// <summary>
        /// Lấy ca làm việc theo Id
        /// </summary>
        /// <param name="workShiftId">Id ca làm việc</param>
        /// <returns>Ca làm việc</returns>
        /// Created by: LQThinh (05/12/2025)
        public async Task<WorkShift> GetWorkShiftByIdAsync(string workShiftId)
        {
            try
            {
                var sql = @"
                SELECT *
                FROM work_shift
                WHERE work_shift_id = @Id;";

                using (var db = new MySqlConnection(_connectionString))
                {
                    var result = await db.QuerySingleOrDefaultAsync<WorkShift>(sql, new { Id = workShiftId });
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Kiểm tra ca làm việc đã tồn tại hay chưa
        /// </summary>
        /// <param name="workShiftCode"></param>
        /// <returns>true/false</returns>
        /// Created by: LQThinh (04/12/2025)
        public bool IsWorkShiftExists(string workShiftCode)
        {
            using (var db = new MySqlConnection(_connectionString))
            {
                var checkWorkShiftCodeSql = "SELECT COUNT(*) FROM work_shift WHERE work_shift_code = @WorkShiftCode";
                var exists = db.ExecuteScalar<int>(checkWorkShiftCodeSql, new { WorkShiftCode = workShiftCode });
                return exists > 0;
            }
        }

        /// <summary>
        /// Kiểm tra workShiftCode đã có hay chưa ngoại trừ excludeId
        /// </summary>
        /// <param name="workShiftCode"></param>
        /// <param name="excludeId"></param>
        /// <returns>true/false</returns>
        /// Created by: LQThinh (05/12/2025)
        public bool IsWorkShiftCodeExists(string workShiftCode, string? excludeCode = null)
        {
            using (var db = new MySqlConnection(_connectionString))
            {
                var sql = @"
                    SELECT COUNT(*) FROM work_shift
                    WHERE work_shift_code = @WorkShiftCode
                      AND (@ExcludeCode IS NULL OR work_shift_code <> @ExcludeCode);";
                var count = db.ExecuteScalar<int>(sql, new { WorkShiftCode = workShiftCode, ExcludeCode = excludeCode });
                return count == 0;
            }
        }

        /// <summary>
        /// Sửa ca làm việc
        /// </summary>
        /// <param name="request"></param>
        /// <param name="workShiftId"></param>
        /// <returns>Ca làm việc vừa sửa</returns>
        /// <exception cref="Exception"></exception>
        /// Created by: LQThinh (05/12/2025)
        public async Task<WorkShift> UpdateWorkShiftAsync(string workShiftId, WorkShiftUpdateRequest request)
        {
            try
            {
                var sql = @"
                UPDATE work_shift SET
                    work_shift_code = @WorkShiftCode,
                    work_shift_name = @WorkShiftName,
                    start_time = @StartTime,
                    end_time = @EndTime,
                    break_start = @BreakStart,
                    break_end = @BreakEnd,
                    working_hours = @WorkingHours,
                    break_hours = @BreakHours,
                    work_shift_status = @WorkShiftStatus,
                    modified_by = @ModifiedBy,
                    modified_date = NOW(),
                    description = @Description
                WHERE work_shift_id = @WorkShiftId;";

                var parameters = new
                {
                    WorkShiftId = workShiftId,
                    WorkShiftCode = request.WorkShiftCode,
                    WorkShiftName = request.WorkShiftName,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    BreakStart = request.BreakStart,
                    BreakEnd = request.BreakEnd,
                    WorkingHours = request.WorkingHours,
                    BreakHours = request.BreakHours,
                    WorkShiftStatus = request.WorkShiftStatus,
                    ModifiedBy = "ThinhLQ",
                    Description = request.Description,
                };

                using (var db = new MySqlConnection(_connectionString))
                {
                    var affected = await db.ExecuteAsync(sql, parameters);

                    var updated = await GetWorkShiftByIdAsync(workShiftId);
                    return updated;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Xóa ca làm việc
        /// </summary>
        /// <param name="workShiftId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// Created by: LQThinh (05/12/2025)
        public async Task<int> DeleteWorkShiftAsync(WorkShiftDeleteDto request)
        {
            try
            {
                using (var db = new MySqlConnection(_connectionString))
                {
                    var sql = "DELETE FROM work_shift WHERE work_shift_id IN @WorkShiftIds;";
                    var parameters = new
                    {
                        WorkShiftIds = request.workShiftIds
                    };
                    var affected = await db.ExecuteAsync(sql, parameters);
                    if (affected == 0)
                    {
                        throw new Exception("Xóa ca không thành công");
                    }
                    return affected;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Cập nhật trạng thái ca
        /// </summary>
        /// <param name="workShiftIds"></param>
        /// <param name="newStatus"></param>
        /// <returns>Số lượng ca đc cập nhật</returns>
        public async Task<int> UpdateMultipleStatusAsync(UpdateStatusDto request)
        {
            string sql = @"
            UPDATE work_shift 
            SET work_shift_status = @NewStatus
            WHERE work_shift_id IN @WorkShiftIds;";
            var parameters = new
            {
                NewStatus = request.newStatus,
                WorkShiftIds = request.workShiftIds
            };

            using (var db = new MySqlConnection(_connectionString))
            {
                int affectedRows = await db.ExecuteAsync(sql, parameters);
                return affectedRows;
            }
        }
    }
}
