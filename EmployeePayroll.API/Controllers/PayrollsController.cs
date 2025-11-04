
using AutoMapper;
using EmployeePayroll.Application.DTOs;
using EmployeePayroll.Core.Entities;
using EmployeePayroll.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeePayroll.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PayrollsController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IPayrollService _payrollService;
        private readonly IMapper _mapper;
        private readonly ILogger<PayrollsController> _logger;

        public PayrollsController(IEmployeeRepository employeeRepository,
                                IPayrollService payrollService,
                                IMapper mapper,
                                ILogger<PayrollsController> logger)
        {
            _employeeRepository = employeeRepository;
            _payrollService = payrollService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PayrollDto>>> GetPayrolls([FromQuery] string month)
        {
            try
            {
                _logger.LogInformation("Getting payrolls for month: {Month}", month);

                if (!DateTime.TryParseExact(month, "yyyy-MM", null, DateTimeStyles.None, out var date))
                    return BadRequest("Invalid month format. Use yyyy-MM");

                var employees = await _employeeRepository.GetEmployeesWithPayrollAsync(date.Month, date.Year);

                var payrolls = employees.Select(employee =>
                {
                    var workLog = employee.WorkLogs?.FirstOrDefault();
                    var salary = _payrollService.CalculateSalary(employee, workLog ?? new WorkLog());

                    var payroll = _mapper.Map<PayrollDto>(employee);
                    payroll.CalculatedSalary = salary;
                    payroll.Month = date.Month;
                    payroll.Year = date.Year;

                    return payroll;
                }).ToList(); 

                return Ok(payrolls);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payrolls for month: {Month}", month);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}