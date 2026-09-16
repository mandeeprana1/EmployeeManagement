using EmployeeManagement.DTOs;
using Microsoft.AspNetCore.Authorization;
using EmployeeManagement.Models;
using EmployeeManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace EmployeeManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly AppDbContext _context;
        public EmployeeController(AppDbContext context)
        {
            _context = context;
        }

        private static EmployeeResponseDto MapEmployee(Employee employee)
        {
            return new EmployeeResponseDto
            {
                Id = employee.Id,
                FullName = employee.FullName,
                EmailAddress = employee.EmailAddress,
                PhoneNumber = employee.PhoneNumber,
                Salary = employee.Salary,
                DateOfJoining = employee.DateOfJoining,
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.Department?.DepartmentName ?? string.Empty
            };
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddEmployee(CreateEmployeeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var departmentExists=await _context.Departments
                .AnyAsync(d=>d.Id==dto.DepartmentId);
            if (!departmentExists)
            {
                return BadRequest(new { message = "Selected Department does not exists" });
            }
            var emailExists=await _context.Employees
                .AnyAsync(e=>e.EmailAddress==dto.EmailAddress);
            if (emailExists)
            {
                return Conflict(new { message = "An Employee With This email Alredy Exists" });
            }
            var employee = new Employee
            {
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                EmailAddress = dto.EmailAddress,
                Salary = dto.Salary,
                DateOfJoining = dto.DateOfJoining,
                DepartmentId = dto.DepartmentId
            };
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            var createdEmployee = await _context.Employees
     .Include(e => e.Department)
     .FirstAsync(e => e.Id == employee.Id);

            return StatusCode(StatusCodes.Status201Created, MapEmployee(createdEmployee));
        }
        [HttpGet]
        public async Task<IActionResult> GetEmployees(
         string? search = null,
         int? departmentId = null,
         string? sortBy = null,
         bool descending = false,
         int pageNumber = 1,
         int pageSize = 5)
        {
            var query = _context.Employees
                .Include(e => e.Department)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var normalizedSearch = search.Trim().ToLower();

                query = query.Where(e =>
                    e.FullName.ToLower().Contains(normalizedSearch) ||
                    e.EmailAddress.ToLower().Contains(normalizedSearch));
            }

            if (departmentId.HasValue)
            {
                query = query.Where(e => e.DepartmentId == departmentId.Value);
            }

            query = sortBy?.ToLower() switch
            {
                "name" => descending
                    ? query.OrderByDescending(e => e.FullName)
                    : query.OrderBy(e => e.FullName),

                "salary" => descending
                    ? query.OrderByDescending(e => e.Salary)
                    : query.OrderBy(e => e.Salary),

                "date" => descending
                    ? query.OrderByDescending(e => e.DateOfJoining)
                    : query.OrderBy(e => e.DateOfJoining),

                _ => query.OrderBy(e => e.Id)
            };

            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            if (pageSize < 1 || pageSize > 50)
            {
                pageSize = 5;
            }

            var totalRecords = await query.CountAsync();

            var employees = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                totalRecords,
                pageNumber,
                pageSize,
                totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
                data = employees.Select(MapEmployee)
            });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound(new { message = "Employee not found" });
            }

            return Ok(MapEmployee(employee));
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id,UpdateEmployeeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) {
                return NotFound(new { message = "Employee Not Found" });
            }
            var departmentExist=await _context.Departments
                .AnyAsync(d => d.Id == dto.DepartmentId);
            if (!departmentExist)
            {
                return BadRequest(new { message = "Select Department Not Exists" });
            }
            var emailExists = await _context.Employees
     .AnyAsync(e => e.EmailAddress == dto.EmailAddress && e.Id != id);

            if (emailExists)
            {
                return Conflict(new
                {
                    message = "An employee with this email already exists"
                });
            }





            employee.FullName = dto.FullName;
            employee.EmailAddress = dto.EmailAddress;
            employee.PhoneNumber = dto.PhoneNumber;
            employee.Salary = dto.Salary;
            employee.DateOfJoining = dto.DateOfJoining;
            employee.DepartmentId = dto.DepartmentId;
            await _context.SaveChangesAsync();
            var updatedEmployee = await _context.Employees
      .Include(e => e.Department)
      .FirstAsync(e => e.Id == employee.Id);

            return Ok(MapEmployee(updatedEmployee));
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult>DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if(employee == null)
            {
                return NotFound(new { message = "Employee Not Found" });
            }
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Employee Deleted Successfuly " });
        }
       

    } 
}
