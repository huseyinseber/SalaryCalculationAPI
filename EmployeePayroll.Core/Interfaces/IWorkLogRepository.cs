using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeePayroll.Core.Entities;

namespace EmployeePayroll.Core.Interfaces
{
    public interface IWorkLogRepository : IRepository<WorkLog>
    {
        Task<WorkLog?> GetByEmployeeAndPeriodAsync(int employeeId, int month, int year);
        Task<IEnumerable<WorkLog>> GetWorkLogsByPeriodAsync(int month, int year);
    }
}
