using System.ComponentModel.DataAnnotations;

namespace payroll_mvc.Areas.Admin.ViewModels
{
    public class DepartmentViewModel
    {
        public Guid DeptId { get; set; }
        public string? DeptCode { get; set; }
        public string? DeptName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
