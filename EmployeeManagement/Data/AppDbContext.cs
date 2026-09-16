using EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore;
namespace EmployeeManagement.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        public DbSet<Employee> Employees { get; set; }

        public DbSet<Department>Departments { get; set; }

        public DbSet<AppUser> Users { get; set; }
    }
}
