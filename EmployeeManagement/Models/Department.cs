using System.ComponentModel.DataAnnotations;
namespace EmployeeManagement.Models
{
    public class Department
    {
        public int Id { get; set;  }

        [Required]
        [MaxLength(100)]
        public string DepartmentName { get; set;  }=string.Empty;

    }
}
