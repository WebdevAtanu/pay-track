namespace payroll_mvc.Areas.Admin.Models
{
    public class EmployeeViewModel
    {
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
