namespace payroll_mvc.ViewModels
{
    public class DashboardViewModel
    {

        public int EmployeeCount { get; set; }
        public decimal? TotalSalary { get; set; }
        public decimal? SettledSalary { get; set; }
        public decimal? PendingSalary { get; set; }
    }
}
