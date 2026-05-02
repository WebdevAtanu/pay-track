using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace payroll_mvc.Models
{
    [Table("Map_Department")]
    public class MapDepartment
    {
        [Key]
        public Guid MapId { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? DeptId { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
