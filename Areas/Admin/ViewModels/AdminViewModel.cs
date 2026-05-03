using System.ComponentModel.DataAnnotations;

namespace payroll_mvc.Areas.Admin.ViewModels
{
    public class DetailedEmployeeViewModel
    {
        public Guid EmployeeId { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public DateTime? JoiningDate { get; set; }
        public bool? IsActive { get; set; }
        public EmployeeDepartment? EmployeeDepartment { get; set; }
        public List<EmployeeAttendance>? EmployeeAttendances { get; set; }
        public List<EmployeeSalary>? EmployeeSalaries { get; set; }
    }

    public class EmployeeDepartment
    {
        public Guid DeptId { get; set; }
        public string? DeptName { get; set; }
        public bool IsActive { get; set; }
    }

    public class EmployeeAttendance
    {
        public Guid AttendanceId { get; set; }
        public DateTime? Date { get; set; }
        public string? Status { get; set; }
    }

    public class EmployeeSalary
    {
        public Guid SalaryId { get; set; }
        public string? Month { get; set; }
        public string? Year { get; set; }
        public decimal? Basic { get; set; }
        public decimal? HRA { get; set; }
        public decimal? Bonus { get; set; }
        public decimal? Deduction { get; set; }
        public decimal? NetSalary { get; set; }
        public string? Status { get; set; }
    }
}
