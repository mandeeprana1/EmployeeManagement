using EmployeeManagement.Data;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace EmployeeManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DepartmentController(AppDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetDepartment()
        {
            var department = await _context.Departments.ToListAsync();
            return Ok(department);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddDepartment(Department department)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
            return StatusCode(StatusCodes.Status201Created, department);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDepartmentById(int id)
        {
            var department = await _context.Departments.FindAsync(id);

            if (department == null)
            {
                return NotFound(new { message = "Department not found" });
            }

            return Ok(department);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, Department updatedDepartment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var department = await _context.Departments.FindAsync(id);

            if (department == null)
            {
                return NotFound(new { message = "Department not found" });
            }

            department.DepartmentName = updatedDepartment.DepartmentName;
            await _context.SaveChangesAsync();

            return Ok(department);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var department = await _context.Departments.FindAsync(id);

            if (department == null)
            {
                return NotFound(new { message = "Department not found" });
            }

            var hasEmployees = await _context.Employees
                .AnyAsync(e => e.DepartmentId == id);

            if (hasEmployees)
            {
                return BadRequest(new
                {
                    message = "Cannot delete a department with assigned employees"
                });
            }

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Department deleted successfully" });
        }
    }
}
