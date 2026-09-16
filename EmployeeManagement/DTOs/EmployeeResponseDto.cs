namespace EmployeeManagement.DTOs
{
    public class EmployeeResponseDto
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string EmailAddress { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public decimal Salary { get; set; }

        public DateTime DateOfJoining { get; set; }

        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; } = string.Empty;
    }
}