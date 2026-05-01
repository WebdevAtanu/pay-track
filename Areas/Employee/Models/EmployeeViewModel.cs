namespace payroll_mvc.Areas.Admin.Models
{
    public class AttendanceViewModel
    {
        public Guid EmployeeId { get; set; }
        public string? EmpCode { get; set; }
        public string? Name { get; set; }
        public DateTime? Date { get; set; }
        public string? Status { get; set; }
        public string? Note { get; set; }
    }
}
