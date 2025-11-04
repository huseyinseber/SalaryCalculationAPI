using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePayroll.Application.DTOs
{
    public class CreateWorkLogDto
    {
        public int EmployeeId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int WorkedDays { get; set; }
        public int OvertimeHours { get; set; }
    }
}
