using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace payroll_mvc.Models
{
    [Table("Salary")]
    public class Salary
    {
        [Key]
        public Guid SalaryId { get; set; }
        public Guid EmployeeId { get; set; }
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
