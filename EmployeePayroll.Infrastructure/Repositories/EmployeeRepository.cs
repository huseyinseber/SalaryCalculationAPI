using Dapper;
using EmployeePayroll.Core.Entities;
using EmployeePayroll.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace EmployeePayroll.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IDbConnection _connection;
        private readonly ILogger<EmployeeRepository> _logger;

        public EmployeeRepository(IDbConnection connection, ILogger<EmployeeRepository> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Getting employee by id: {Id}", id);
                const string sql = "SELECT * FROM Employees WHERE Id = @Id";
                return await _connection.QueryFirstOrDefaultAsync<Employee>(sql, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employee by id: {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Getting all employees");
                const string sql = "SELECT * FROM Employees";
                return await _connection.QueryAsync<Employee>(sql);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all employees");
                throw;
            }
        }

        public async Task AddAsync(Employee employee)
        {
            try
            {
                _logger.LogInformation("Adding new employee: {TCKN}", employee.TCKN);
                const string sql = @"INSERT INTO Employees (TCKN, FirstName, LastName, Type, BaseSalary, DailyRate, OvertimeRate) 
                                   VALUES (@TCKN, @FirstName, @LastName, @Type, @BaseSalary, @DailyRate, @OvertimeRate);
                                   SELECT CAST(SCOPE_IDENTITY() as int)";

                var id = await _connection.ExecuteScalarAsync<int>(sql, employee);
                employee.Id = id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding employee: {TCKN}", employee.TCKN);
                throw;
            }
        }

        public async Task UpdateAsync(Employee employee)
        {
            try
            {
                _logger.LogInformation("Updating employee: {Id}", employee.Id);
                const string sql = @"UPDATE Employees SET TCKN = @TCKN, FirstName = @FirstName, LastName = @LastName,
                                   Type = @Type, BaseSalary = @BaseSalary, DailyRate = @DailyRate, OvertimeRate = @OvertimeRate
                                   WHERE Id = @Id";

                await _connection.ExecuteAsync(sql, employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating employee: {Id}", employee.Id);
                throw;
            }
        }

        public async Task DeleteAsync(Employee employee)
        {
            try
            {
                _logger.LogInformation("Deleting employee: {Id}", employee.Id);
                const string sql = "DELETE FROM Employees WHERE Id = @Id";
                await _connection.ExecuteAsync(sql, new { employee.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting employee: {Id}", employee.Id);
                throw;
            }
        }

        public async Task<Employee?> GetByTCKNAsync(string tckn)
        {
            try
            {
                _logger.LogInformation("Getting employee by TCKN: {TCKN}", tckn);
                const string sql = "SELECT * FROM Employees WHERE TCKN = @TCKN";
                return await _connection.QueryFirstOrDefaultAsync<Employee>(sql, new { TCKN = tckn });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employee by TCKN: {TCKN}", tckn);
                throw;
            }
        }

        public async Task<IEnumerable<Employee>> GetEmployeesWithPayrollAsync(int month, int year)
        {
            try
            {
                _logger.LogInformation("Getting employees with payroll for {Month}/{Year}", month, year);

                var parameters = new { Month = month, Year = year };
                var employees = await _connection.QueryAsync<Employee>(
                    "sp_GetEmployeesWithPayroll",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                foreach (var employee in employees)
                {
                    var workLog = await _connection.QueryFirstOrDefaultAsync<WorkLog>(
                        "SELECT * FROM WorkLogs WHERE EmployeeId = @EmployeeId AND Month = @Month AND Year = @Year",
                        new { EmployeeId = employee.Id, Month = month, Year = year });

                    employee.WorkLogs = workLog != null ? new List<WorkLog> { workLog } : new List<WorkLog>();
                }

                return employees;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employees with payroll");
                throw;
            }
        }
    }
}