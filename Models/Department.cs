using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace payroll_mvc.Models
{
    [Table("Department")]
    public class Department
    {
        [Key]
        public Guid DeptId { get; set; }
        public string? DeptName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
