using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.DTOs
{
    public class UpdateEmployeeDto
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string EmailAddress { get; set; } = string.Empty;

        [MaxLength(15)]
        public string? PhoneNumber { get; set; }

        [Range(0, 99999999)]
        public decimal Salary { get; set; }

        public DateTime DateOfJoining { get; set; }

        [Range(1, int.MaxValue)]
        public int DepartmentId { get; set; }
    }
}