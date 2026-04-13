using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace payroll_mvc.Models
{
    [Table("Employee")]
    public class Employee
    {
        [Key]
        public Guid EmployeeId { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public Guid? DeptId { get; set; }
        public DateTime? JoiningDate { get; set; }
        public decimal? BasicSalary { get; set; }
        public bool? IsActive { get; set; }
    }
}
