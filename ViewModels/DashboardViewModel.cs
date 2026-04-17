namespace payroll_mvc.ViewModels
{
    public class DashboardViewModel
    {
        public int EmployeeCount { get; set; }
        public decimal? TotalSalary { get; set; }
        public decimal? SettledSalary { get; set; }
        public decimal? PendingSalary { get; set; }
        public List<RecentPayrolls>? RecentPayrolls { get; set; }
    }

    public class CardData
    {
        public string? IconClass { get; set; }
        public string? Title { get; set; }
        public string? Value { get; set; }
        public string? ColorClass { get; set; }
    }

    public class RecentPayrolls
    {
        public int EmployeeCount { get; set; }
        public string? Name { get; set; }
        public string? Month { get; set; }
        public decimal? Gross { get; set; }
        public decimal? Deduction { get; set; }
        public decimal? Net { get; set; }
    }
}
