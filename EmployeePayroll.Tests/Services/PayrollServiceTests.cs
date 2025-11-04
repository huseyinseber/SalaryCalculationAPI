using EmployeePayroll.Application.Services;
using EmployeePayroll.Core.Entities;
using EmployeePayroll.Core.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EmployeePayroll.Tests.Services
{
    public class PayrollServiceTests
    {
        private readonly PayrollService _payrollService;
        private readonly Mock<ILogger<PayrollService>> _loggerMock;

        public PayrollServiceTests()
        {
            _loggerMock = new Mock<ILogger<PayrollService>>();
            _payrollService = new PayrollService(_loggerMock.Object);
        }

        [Theory]
        [InlineData(EmployeeType.Fixed, 5000, 0, 0, 5000)]
        [InlineData(EmployeeType.Daily, 0, 20, 0, 4000)]
        [InlineData(EmployeeType.FixedWithOvertime, 4000, 0, 10, 4500)]
        public void CalculateSalary_ValidInputs_ReturnsCorrectSalary(
            EmployeeType type, decimal baseSalary, int workedDays, int overtimeHours, decimal expected)
        {
            // Arrange
            var employee = new Employee
            {
                Type = type,
                BaseSalary = baseSalary,
                DailyRate = 200,
                OvertimeRate = 50
            };
            var workLog = new WorkLog
            {
                WorkedDays = workedDays,
                OvertimeHours = overtimeHours
            };

            // Act
            var result = _payrollService.CalculateSalary(employee, workLog);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CalculateSalary_InvalidEmployeeType_ThrowsArgumentException()
        {
      
            var employee = new Employee { Type = (EmployeeType)99 };
            var workLog = new WorkLog();

     
            var exception = Assert.Throws<ArgumentException>(() =>
                _payrollService.CalculateSalary(employee, workLog));
        }

     
        [Fact]
        public void CalculateSalary_FixedEmployee_WithEmptyWorkLog_ReturnsBaseSalary()
        {
    
            var employee = new Employee
            {
                Type = EmployeeType.Fixed,
                BaseSalary = 5000
            };
            var emptyWorkLog = new WorkLog(); // Boş workLog kullan

        
            var result = _payrollService.CalculateSalary(employee, emptyWorkLog);

        
            Assert.Equal(5000, result);
        }

        [Fact]
        public void CalculateSalary_DailyEmployee_WithEmptyWorkLog_ReturnsZero()
        {
            // Arrange
            var employee = new Employee
            {
                Type = EmployeeType.Daily,
                DailyRate = 200
            };
            var emptyWorkLog = new WorkLog(); // Boş workLog kullan

            // Act
            var result = _payrollService.CalculateSalary(employee, emptyWorkLog);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void CalculateSalary_FixedWithOvertimeEmployee_WithEmptyWorkLog_ReturnsBaseSalary()
        {
       
            var employee = new Employee
            {
                Type = EmployeeType.FixedWithOvertime,
                BaseSalary = 4000,
                OvertimeRate = 50
            };
            var emptyWorkLog = new WorkLog(); // Boş workLog kullan

         
            var result = _payrollService.CalculateSalary(employee, emptyWorkLog);

       
            Assert.Equal(4000, result);
        }

        //// Skip edilen testi kaldır veya düzelt
        //[Fact(Skip = "Null WorkLog behavior needs clarification")]
        //public void CalculateSalary_NullWorkLog_CalculatesWithDefaultValues()
        //{
        //    //  skip 
        //}
    }
}