using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeePayroll.Core.Enums;

namespace EmployeePayroll.Application.DTOs
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string TCKN { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public EmployeeType Type { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal DailyRate { get; set; }
        public decimal OvertimeRate { get; set; }
    }
}
