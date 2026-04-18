using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using payroll_mvc.Data;
using payroll_mvc.Models;
using payroll_mvc.ViewModels;

namespace payroll_mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDBContext _context;

        public HomeController(ILogger<HomeController> logger, AppDBContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            var employeeCount = _context.Employees.Count();

            var totalSalary = _context.Salaries
                .Sum(s => (decimal?)s.NetSalary) ?? 0;

            var settledSalary = _context.Salaries
                .Where(s => s.Status != null && s.Status.ToLower() == "settled")
                .Sum(s => (decimal?)s.NetSalary) ?? 0;

            var pendingSalary = _context.Salaries
                .Where(s => s.Status != null && s.Status.ToLower() == "pending")
                .Sum(s => (decimal?)s.NetSalary) ?? 0;

            var recentPayroll = (from s in _context.Salaries
                                 where s.Status != null && s.Status.ToLower() == "settled"
                                 orderby s.Month descending

                                 join e in _context.Employees
                                 on s.EmployeeId equals e.EmployeeId into empSalary
                                 from es in empSalary.DefaultIfEmpty()                        

                                 select new RecentPayrolls
                                 {
                                     Name = es != null ? es.Name : "Unknown",
                                     Month = s.Month,
                                     Gross = s.Basic,
                                     Deduction = s.Deduction,
                                     Net = s.NetSalary
                                 }).Take(10);

            var model = new DashboardViewModel
            {
                EmployeeCount = employeeCount,
                TotalSalary = totalSalary,
                SettledSalary = settledSalary,
                PendingSalary = pendingSalary,
                RecentPayrolls = recentPayroll.ToList()
            };

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
