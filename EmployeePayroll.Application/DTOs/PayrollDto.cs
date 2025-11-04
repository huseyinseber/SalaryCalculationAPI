using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeePayroll.Core.Enums;

namespace EmployeePayroll.Application.DTOs
{
    public class PayrollDto
    {
        public string EmployeeName { get; set; }
        public string MaskedTCKN { get; set; }
        public EmployeeType Type { get; set; }
        public decimal CalculatedSalary { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
