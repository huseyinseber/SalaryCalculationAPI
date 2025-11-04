using Dapper;
using EmployeePayroll.Core.Entities;
using EmployeePayroll.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace EmployeePayroll.Infrastructure.Repositories
{
    public class WorkLogRepository : IWorkLogRepository
    {
        private readonly IDbConnection _connection;
        private readonly ILogger<WorkLogRepository> _logger;

        public WorkLogRepository(IDbConnection connection, ILogger<WorkLogRepository> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        public async Task<WorkLog?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Getting worklog by id: {Id}", id);
                const string sql = "SELECT * FROM WorkLogs WHERE Id = @Id";
                return await _connection.QueryFirstOrDefaultAsync<WorkLog>(sql, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting worklog by id: {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<WorkLog>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Getting all worklogs");
                const string sql = "SELECT * FROM WorkLogs";
                return await _connection.QueryAsync<WorkLog>(sql);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all worklogs");
                throw;
            }
        }

        public async Task AddAsync(WorkLog workLog)
        {
            try
            {
                _logger.LogInformation("Adding new worklog for employee: {EmployeeId}", workLog.EmployeeId);
                const string sql = @"INSERT INTO WorkLogs (EmployeeId, Month, Year, WorkedDays, OvertimeHours) 
                                   VALUES (@EmployeeId, @Month, @Year, @WorkedDays, @OvertimeHours);
                                   SELECT CAST(SCOPE_IDENTITY() as int)";

                var id = await _connection.ExecuteScalarAsync<int>(sql, workLog);
                workLog.Id = id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding worklog for employee: {EmployeeId}", workLog.EmployeeId);
                throw;
            }
        }

        public async Task UpdateAsync(WorkLog workLog)
        {
            try
            {
                _logger.LogInformation("Updating worklog: {Id}", workLog.Id);
                const string sql = @"UPDATE WorkLogs SET EmployeeId = @EmployeeId, Month = @Month, Year = @Year,
                                   WorkedDays = @WorkedDays, OvertimeHours = @OvertimeHours 
                                   WHERE Id = @Id";

                await _connection.ExecuteAsync(sql, workLog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating worklog: {Id}", workLog.Id);
                throw;
            }
        }

        public async Task DeleteAsync(WorkLog workLog)
        {
            try
            {
                _logger.LogInformation("Deleting worklog: {Id}", workLog.Id);
                const string sql = "DELETE FROM WorkLogs WHERE Id = @Id";
                await _connection.ExecuteAsync(sql, new { workLog.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting worklog: {Id}", workLog.Id);
                throw;
            }
        }

        public async Task<WorkLog?> GetByEmployeeAndPeriodAsync(int employeeId, int month, int year)
        {
            try
            {
                _logger.LogInformation("Getting worklog for employee {EmployeeId} period {Month}/{Year}",
                    employeeId, month, year);

                const string sql = "SELECT * FROM WorkLogs WHERE EmployeeId = @EmployeeId AND Month = @Month AND Year = @Year";
                return await _connection.QueryFirstOrDefaultAsync<WorkLog>(sql, new
                {
                    EmployeeId = employeeId,
                    Month = month,
                    Year = year
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting worklog for employee {EmployeeId} period {Month}/{Year}",
                    employeeId, month, year);
                throw;
            }
        }

        public async Task<IEnumerable<WorkLog>> GetWorkLogsByPeriodAsync(int month, int year)
        {
            try
            {
                _logger.LogInformation("Getting worklogs for period {Month}/{Year}", month, year);

                const string sql = "SELECT * FROM WorkLogs WHERE Month = @Month AND Year = @Year";
                return await _connection.QueryAsync<WorkLog>(sql, new { Month = month, Year = year });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting worklogs for period {Month}/{Year}", month, year);
                throw;
            }
        }
    }
}