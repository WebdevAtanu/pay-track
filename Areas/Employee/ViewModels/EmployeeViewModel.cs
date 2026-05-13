using payroll_mvc.Models;

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
        public string? FaceDescriptor { get; set; }
        public bool? IsActive { get; set; }
        public List<Department>? Departments { get; set; }
    }

    public class EmpFaceData
    {
        public Guid EmployeeId { get; set; }
        public string Descriptor { get; set; } = "";
        public string? EmpCode { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public DateTime? Date { get; set; }
    }
}
