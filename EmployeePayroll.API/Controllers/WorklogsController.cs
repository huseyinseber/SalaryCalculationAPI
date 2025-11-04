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
    public class WorklogsController : ControllerBase
    {
        private readonly IWorkLogRepository _workLogRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<WorklogsController> _logger;

        public WorklogsController(IWorkLogRepository workLogRepository,
                                 IEmployeeRepository employeeRepository,
                                 IMapper mapper,
                                 ILogger<WorklogsController> logger)
        {
            _workLogRepository = workLogRepository;
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<WorkLogDto>> CreateWorkLog([FromBody] CreateWorkLogDto createWorkLogDto)
        {
            try
            {
                _logger.LogInformation("Creating worklog for employee: {EmployeeId}", createWorkLogDto.EmployeeId);

                // Employee var mı kontrl et....
                var employee = await _employeeRepository.GetByIdAsync(createWorkLogDto.EmployeeId);
                if (employee == null)
                    return BadRequest("Employee not found");

                // Aynı dönem için wrklg var mı kontrol et..
                var existingWorkLog = await _workLogRepository.GetByEmployeeAndPeriodAsync(
                    createWorkLogDto.EmployeeId, createWorkLogDto.Month, createWorkLogDto.Year);

                if (existingWorkLog != null)
                    return BadRequest("Worklog already exists for this period");

                var workLog = _mapper.Map<WorkLog>(createWorkLogDto);
                await _workLogRepository.AddAsync(workLog);

                var result = _mapper.Map<WorkLogDto>(workLog);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating worklog for employee: {EmployeeId}", createWorkLogDto.EmployeeId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("employee/{employeeId}/period/{month}/{year}")]
        public async Task<ActionResult<WorkLogDto>> GetWorkLog(int employeeId, int month, int year)
        {
            try
            {
                _logger.LogInformation("Getting worklog for employee {EmployeeId} period {Month}/{Year}",
                    employeeId, month, year);

                var workLog = await _workLogRepository.GetByEmployeeAndPeriodAsync(employeeId, month, year);
                if (workLog == null)
                    return NotFound();

                var result = _mapper.Map<WorkLogDto>(workLog);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting worklog for employee {EmployeeId} period {Month}/{Year}",
                    employeeId, month, year);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
