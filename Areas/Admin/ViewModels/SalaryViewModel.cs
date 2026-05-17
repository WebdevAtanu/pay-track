using System.ComponentModel.DataAnnotations;

namespace payroll_mvc.Areas.Admin.ViewModels
{
    public class SalaryViewModel
    {
        public Guid SalaryId { get; set; }
        public Guid EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
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
