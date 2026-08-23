using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using payroll_mvc.Data;
using payroll_mvc.Entities;
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
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard", "Home");
            }
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
            // Validate login type selection
            if (string.IsNullOrEmpty(model.LoginType))
            {
                ModelState.AddModelError("", "Please select login type");
                return View("Index", model);
            }

            Admin? admin = null;
            Employee? employee = null;

            // Check user based on login type
            if (model.LoginType == "Admin")
            {
                admin = _context.Admins.FirstOrDefault(u => u.Email == model.Email);
                if (admin == null)
                {
                    ModelState.AddModelError("", "Invalid admin credentials");
                    return View("Index", model);
                }

                if (!BCrypt.Net.BCrypt.Verify(model.Password, admin.Password))
                {
                    ModelState.AddModelError("", "Invalid admin credentials");
                    return View("Index", model);
                }
            }
            else if (model.LoginType == "Employee")
            {
                employee = _context.Employees.FirstOrDefault(e => e.Email == model.Email);
                if (employee == null)
                {
                    ModelState.AddModelError("", "Invalid employee credentials");
                    return View("Index", model);
                }

                if (!BCrypt.Net.BCrypt.Verify(model.Password, employee.Password))
                {
                    ModelState.AddModelError("", "Invalid employee credentials");
                    return View("Index", model);
                }

                // Auto-mark attendance for employee on login
                await MarkEmployeeAttendance(employee.EmployeeId);
            }
            else
            {
                ModelState.AddModelError("", "Invalid login type selected");
                return View("Index", model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, model.Email ?? ""),
                new Claim(ClaimTypes.Name, admin?.Name ?? employee?.Name ?? ""),
                new Claim(ClaimTypes.Role, model.LoginType)
            };

            if (employee != null)
            {
                claims.Add(new Claim("EmployeeId", employee.EmployeeId.ToString()));
            }

            var identity = new ClaimsIdentity(claims, "userAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("userAuth", principal);

            return model.LoginType == "Admin" ? RedirectToAction("Dashboard", "Home") :
                RedirectToAction(
                actionName: "Index",
                controllerName: "EmployeeDashboard",
                routeValues: new { area = "Employee" }
                );
        }

        private async Task MarkEmployeeAttendance(Guid employeeId)
        {
            var today = DateTime.Today;

            // Check if attendance already exists for today
            var existingAttendance = await _context.Attendances
                .FirstOrDefaultAsync(a => a.EmployeeId == employeeId &&
                                        a.Date.HasValue &&
                                        a.Date.Value.Date == today);

            if (existingAttendance == null)
            {
                // Create new attendance record
                var attendance = new Attendance
                {
                    AttendanceId = Guid.NewGuid(),
                    EmployeeId = employeeId,
                    Date = today,
                    Status = "Present",
                    Note = "Auto-marked on login"
                };

                _context.Attendances.Add(attendance);
                await _context.SaveChangesAsync();
            }
        }

        // LOGOUT
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("userAuth");
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
