using System.ComponentModel.DataAnnotations;
namespace EmployeeManagement.DTOs
{
    public class RegisterDto
    {
        [Required]
        [MaxLength(50)]
        public string UserName {  get; set; }=string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; }=string.Empty;  

    }
}
