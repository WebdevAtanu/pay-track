namespace payroll_mvc.Areas.Admin.ViewModels
{
    public class EmployeeViewModel
    {
        public Guid EmployeeId { get; set; }
        public string? EmpCode { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public Guid? DeptId { get; set; }
        public string? DepartmentName { get; set; }
        public DateTime? JoiningDate { get; set; }
        public bool? IsActive { get; set; }
    }
}
