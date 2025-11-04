using EmployeePayroll.Core.Entities;
using EmployeePayroll.Core.Enums;
using EmployeePayroll.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace EmployeePayroll.Application.Services
{
    public class PayrollService : IPayrollService
    {
        private readonly ILogger<PayrollService> _logger;

        public PayrollService(ILogger<PayrollService> logger)
        {
            _logger = logger;
        }

        public decimal CalculateSalary(Employee employee, WorkLog workLog)
        {
            try
            {
                //Çalışan maaşının hesaplanması
                _logger.LogInformation("Calculating salary for employee {EmployeeId}", employee?.Id);

                // Null-safe workLog handling
                var safeWorkLog = workLog ?? new WorkLog();
                var workedDays = safeWorkLog.WorkedDays;
                var overtimeHours = safeWorkLog.OvertimeHours;

                return employee.Type switch
                {
                    EmployeeType.Fixed => employee.BaseSalary,
                    EmployeeType.Daily => workedDays * employee.DailyRate,
                    EmployeeType.FixedWithOvertime => employee.BaseSalary + (overtimeHours * employee.OvertimeRate),
                    _ => throw new ArgumentException($"Invalid employee type: {employee.Type}")
                };
            }
            //Maas hesaplaması hatası
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating salary for employee {EmployeeId}", employee?.Id);
                throw;
            }
        }
    }
}