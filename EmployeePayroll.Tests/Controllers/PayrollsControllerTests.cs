using AutoMapper;
using EmployeePayroll.API.Controllers;
using EmployeePayroll.Application.DTOs;
using EmployeePayroll.Core.Entities;
using EmployeePayroll.Core.Enums;
using EmployeePayroll.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace EmployeePayroll.Tests.Controllers
{
    public class PayrollsControllerTests
    {
        private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
        private readonly Mock<IPayrollService> _payrollServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<PayrollsController>> _loggerMock;
        private readonly PayrollsController _controller;

        public PayrollsControllerTests()
        {
            _employeeRepositoryMock = new Mock<IEmployeeRepository>();
            _payrollServiceMock = new Mock<IPayrollService>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<PayrollsController>>();
            _controller = new PayrollsController(
                _employeeRepositoryMock.Object,
                _payrollServiceMock.Object,
                _mapperMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task GetPayrolls_ValidMonth_ReturnsPayrolls()
        {
            // Arrange
            var month = "2024-09";
            var employees = new List<Employee>
            {
                new Employee
                {
                    Id = 1,
                    FirstName = "Ahmet",
                    LastName = "Yılmaz",
                    TCKN = "12345678901",
                    Type = EmployeeType.Fixed,
                    WorkLogs = new List<WorkLog>()
                }
            };

            _employeeRepositoryMock.Setup(repo => repo.GetEmployeesWithPayrollAsync(9, 2024))
                .ReturnsAsync(employees);

            _payrollServiceMock.Setup(service => service.CalculateSalary(It.IsAny<Employee>(), It.IsAny<WorkLog>()))
                .Returns(5000);

            _mapperMock.Setup(mapper => mapper.Map<PayrollDto>(It.IsAny<Employee>()))
                .Returns(new PayrollDto
                {
                    EmployeeName = "Ahmet Yılmaz",
                    MaskedTCKN = "*******8901",
                    Type = EmployeeType.Fixed,
                    CalculatedSalary = 5000,
                    Month = 9,
                    Year = 2024
                });

            // Act
            var result = await _controller.GetPayrolls(month);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var payrolls = Assert.IsType<List<PayrollDto>>(okResult.Value);
            Assert.Single(payrolls);
            Assert.Equal(5000, payrolls[0].CalculatedSalary);
        }

        [Fact]
        public async Task GetPayrolls_InvalidMonth_ReturnsBadRequest()
        {
            // Arrange
            var month = "invalid-month";

            // Act
            var result = await _controller.GetPayrolls(month);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPayrolls_RepositoryThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var month = "2024-09";
            _employeeRepositoryMock.Setup(repo => repo.GetEmployeesWithPayrollAsync(9, 2024))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetPayrolls(month);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
    }
}