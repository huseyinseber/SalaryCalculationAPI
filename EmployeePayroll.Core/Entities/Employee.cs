using System.Collections.Generic;
using EmployeePayroll.Core.Enums;

namespace EmployeePayroll.Core.Entities
{
    public class Employee
    {
        public Employee()
        {
            WorkLogs = new List<WorkLog>(); // Constructor'da initialize et
        }

        public int Id { get; set; }
        public string TCKN { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public EmployeeType Type { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal DailyRate { get; set; }
        public decimal OvertimeRate { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<WorkLog> WorkLogs { get; set; }
    }
}