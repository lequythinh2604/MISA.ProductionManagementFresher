using Dapper;
using Microsoft.Extensions.Configuration;
using MISA.Core.Dtos;
using MISA.Core.Entities;
using MISA.Core.Interfaces.Repository;
using MISA.Core.Utilities;
using MISA.Infrastructure.Mappings;
using MySqlConnector;
using System.Data;

namespace MISA.Infrastructure.Repositories
{
    public class WorkShiftRepository(IConfiguration configuration) : BaseRepository<WorkShift>(configuration), IWorkShiftRepository
    {
        /// <summary>
        /// Lấy danh sách ca làm việc có hỗ trợ phân trang, tìm kiếm, lọc cột, sắp xếp
        /// </summary>
        /// <param name="request">Đối tượng payload</param>
        /// <returns>Danh sách ca làm việc có hỗ trợ phân trang, tìm kiếm, lọc cột, sắp xếp</returns>
        /// Created by: LQThinh (04/12/2025)
        public async Task<PagedResult<WorkShift>> GetPagingAsync(WorkShiftFilterDto request)
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
            }
            ;

            // lấy mapping cột
            var columnMapping = DbColumnMapping.WorkShiftMapping;

            // Tìm kiếm theo cột
            if (request.FilterColumn.Count > 0)
            {
                var paramIndex = 0;
                foreach (var filter in request.FilterColumn)
                {
                    if (filter == null || string.IsNullOrWhiteSpace(filter.ColumnName)) continue;
                    var paramName = $"@f{paramIndex}";
                    if (columnMapping.TryGetValue(filter.ColumnName, out var dbColumnName))
                    {
                        switch (filter.FilterOperator)
                        {
                            case "isnull":
                                whereClauses.Add($"{dbColumnName} IS NULL");
                                break;
                            case "notnull":
                                whereClauses.Add($"{dbColumnName} IS NOT NULL");
                                break;
                            case "=":
                                whereClauses.Add($"{dbColumnName} = {paramName}");
                                dynamicParameters.Add(paramName, $"{filter.FilterValue}");
                                break;
                            case "<>":
                                whereClauses.Add($"{dbColumnName} <> {paramName}");
                                dynamicParameters.Add(paramName, $"{filter.FilterValue}");
                                break;
                            case "contains":
                                whereClauses.Add($"{dbColumnName} LIKE {paramName}");
                                dynamicParameters.Add(paramName, $"%{filter.FilterValue}%");
                                break;
                            case "notcontains":
                                whereClauses.Add($"{dbColumnName} NOT LIKE {paramName}");
                                dynamicParameters.Add(paramName, $"%{filter.FilterValue}%");
                                break;
                            case "startswith":
                                whereClauses.Add($"{dbColumnName} LIKE {paramName}");
                                dynamicParameters.Add(paramName, $"{filter.FilterValue}%");
                                break;
                            case "endswith":
                                whereClauses.Add($"{dbColumnName} LIKE {paramName}");
                                dynamicParameters.Add(paramName, $"%{filter.FilterValue}");
                                break;
                            case "ACTIVE":
                                whereClauses.Add($"{dbColumnName} = {paramName}");
                                dynamicParameters.Add(paramName, true);
                                break;
                            case "INACTIVE":
                                whereClauses.Add($"{dbColumnName} = {paramName}");
                                dynamicParameters.Add(paramName, false);
                                break;
                            case "<":
                                whereClauses.Add($"{dbColumnName} < {paramName}");
                                dynamicParameters.Add(paramName, $"{filter.FilterValue}");
                                break;
                            case "<=":
                                whereClauses.Add($"{dbColumnName} <= {paramName}");
                                dynamicParameters.Add(paramName, $"{filter.FilterValue}");
                                break;
                            case ">":
                                whereClauses.Add($"{dbColumnName} > {paramName}");
                                dynamicParameters.Add(paramName, $"{filter.FilterValue}");
                                break;
                            case ">=":
                                whereClauses.Add($"{dbColumnName} >= {paramName}");
                                dynamicParameters.Add(paramName, $"{filter.FilterValue}");
                                break;
                            case null:
                                break;
                            default:
                                break;
                        }
                    }
                    paramIndex++;
                }
            }

            // Sắp xếp theo cột
            if (request.SortColumn.Count > 0)
            {
                foreach (var sort in request.SortColumn)
                {
                    if (string.IsNullOrWhiteSpace(sort?.Selector)) continue;
                    if (columnMapping.TryGetValue(sort.Selector, out var dbColumnName))
                    {
                        var direction = sort.Desc ? "DESC" : "ASC";
                        sortClauses.Add($"{dbColumnName} {direction}");
                    }
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
            using (var db = new MySqlConnection(connectionString))
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
        /// Cập nhật thông tin ca làm việc 
        /// </summary>
        /// <param name="id">ID của ca làm việc cần cập nhật</param>
        /// <param name="request">Đối tượng payload</param>
        /// <returns></returns>
        /// Created by: LQThinh (05/12/2025)
        public async Task<WorkShift> UpdateWorkShiftAsync(string id, WorkShiftUpdateDto request)
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
                WorkShiftId = id,
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

            using (var db = new MySqlConnection(connectionString))
            {
                var affected = await db.ExecuteAsync(sql, parameters);

                var updatedEntity = await GetByIdAsync(id);
                return updatedEntity;
            }
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
            string sql = @"
            UPDATE work_shift 
            SET work_shift_status = @NewStatus
            WHERE work_shift_id IN @Ids;";
            var parameters = new
            {
                NewStatus = request.newStatus,
                Ids = request.ids
            };

            using (var db = new MySqlConnection(connectionString))
            {
                int affectedRows = await db.ExecuteAsync(sql, parameters);
                return affectedRows;
            }
        }
    }
}
