using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using payroll_mvc.Data;
using payroll_mvc.Models;
using payroll_mvc.ViewModels;
using System.Diagnostics;
using System.Security.Claims;
using BCrypt.Net;

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

        [HttpPost("register")]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = _context.Admins.FirstOrDefault(u => u.Email == model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Email already exists");
                    return View("Index", model);
                }

                string passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

                Admin admin = new Admin
                {
                    AdminId = Guid.NewGuid(),
                    Name = model.Name,
                    Phone = "",
                    Email = model.Email,
                    Password = passwordHash,
                };

                _context.Admins.Add(admin);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var existingUser = _context.Admins.FirstOrDefault(u => u.Email == model.Email);

            if (existingUser == null)
            {
                ModelState.AddModelError("", "Invalid Details Provided");
                return View("Index", model);
            }

            if (BCrypt.Net.BCrypt.Verify(model.Password, existingUser.Password))
            {
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, model.Email??""),
                new Claim(ClaimTypes.Name, existingUser.Name??"") // optional
            };
                var identity = new ClaimsIdentity(claims, "cookieAuth");
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync("cookieAuth", principal);
                return RedirectToAction("Dashboard", "Home");
            }
            ModelState.AddModelError("", "Invalid Details Provided");
            return View("Index", model);
        }

        // LOGOUT
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("cookieAuth");
            return RedirectToAction("Index");
        }

        [Authorize]
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

            var monthOrder = new List<string>
            {
                "Jan","Feb","Mar","Apr","May","Jun",
                "Jul","Aug","Sep","Oct","Nov","Dec"
            };

            var monthWiseAmount = _context.Salaries
            .GroupBy(s => s.Month)
            .Select(g => new
            {
                Month = g.Key,
                Amount = g.Sum(x => x.NetSalary)
            })
            .ToList()
            .OrderBy(x => monthOrder.IndexOf(x.Month))
            .ToList();

            var chartData = new MonthWiseAmount
            {
                Month = monthWiseAmount.Select(x => x.Month ?? "").ToList(),
                Amount = monthWiseAmount.Select(x => x.Amount ?? 0).ToList()
            };

            var model = new DashboardViewModel
            {
                EmployeeCount = employeeCount,
                TotalSalary = totalSalary,
                SettledSalary = settledSalary,
                PendingSalary = pendingSalary,
                RecentPayrolls = recentPayroll.ToList(),
                MonthWiseAmounts = chartData
            };

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult AccessDenied()
        {
            ViewBag.Message = "You are not authorized to access this page.";
            return View();
        }
    }
}
