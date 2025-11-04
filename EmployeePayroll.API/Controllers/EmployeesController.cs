using AutoMapper;
using EmployeePayroll.Application.DTOs;
using EmployeePayroll.Core.Entities;
using EmployeePayroll.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace EmployeePayroll.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(IEmployeeRepository employeeRepository,
                                  IMapper mapper,
                                  ILogger<EmployeesController> logger)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeDto>> CreateEmployee([FromBody] CreateEmployeeDto createEmployeeDto)
        {
            try
            {
                _logger.LogInformation("Creating new employee with TCKN: {TCKN}", createEmployeeDto.TCKN);

                var employee = _mapper.Map<Employee>(createEmployeeDto);
                await _employeeRepository.AddAsync(employee);

                var result = _mapper.Map<EmployeeDto>(employee);
                return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating employee with TCKN: {TCKN}", createEmployeeDto.TCKN);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> GetEmployee(int id)
        {
            try
            {
                _logger.LogInformation("Getting employee by id: {Id}", id);

                var employee = await _employeeRepository.GetByIdAsync(id);
                if (employee == null)
                    return NotFound();

                var result = _mapper.Map<EmployeeDto>(employee);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employee by id: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees()
        {
            try
            {
                _logger.LogInformation("Getting all employees");

                var employees = await _employeeRepository.GetAllAsync();
                var result = _mapper.Map<IEnumerable<EmployeeDto>>(employees);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all employees");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}