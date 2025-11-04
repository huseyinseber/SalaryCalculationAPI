using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeePayroll.Core.Entities;

namespace EmployeePayroll.Core.Interfaces
{
    public interface IPayrollRepository
    {
        Task<IEnumerable<Employee>> GetEmployeesWithPayrollAsync(int month, int year);
    }
}
