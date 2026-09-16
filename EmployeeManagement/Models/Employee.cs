using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace EmployeeManagement.Models
{
    public class Employee
    {
        public int Id { get; set; }


        [Required]
        [MaxLength(100)]
        public string FullName { get; set;  }=string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength (150)]
        public string EmailAddress { get; set; } = string.Empty;

        [MaxLength(15)]
        public string? PhoneNumber { get; set;  }

        [Range (0, 999999)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Salary {  get; set; }

        public DateTime DateOfJoining { get; set; }
       
        public int DepartmentId { get; set; }

        public Department? Department { get; set; }



    }
}
