using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeePayroll.Core.Entities;

namespace EmployeePayroll.Core.Interfaces
{
    public interface IPayrollService
    {
        decimal CalculateSalary(Employee employee, WorkLog workLog);
    }
}
