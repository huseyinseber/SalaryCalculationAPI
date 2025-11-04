using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePayroll.Core.Entities
{
    public class WorkLog
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int WorkedDays { get; set; }
        public int OvertimeHours { get; set; }
        public DateTime CreatedDate { get; set; }
        public Employee Employee { get; set; }
    }
}
