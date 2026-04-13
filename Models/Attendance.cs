using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace payroll_mvc.Models
{
    [Table("Attendance")]
    public class Attendance
    {
        [Key]
        public Guid AttendanceId { get; set; }
        public Guid EmployeeId { get; set; }
        public DateTime? Date { get; set; }
        public string? Status { get; set; }
    }
}
