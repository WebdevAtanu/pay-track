using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using payroll_mvc.Areas.Admin.ViewModels;
using payroll_mvc.Data;

namespace payroll_mvc.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class AdminController : Controller
    {
        private readonly AppDBContext _context;

        public AdminController(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var data = await (from e in _context.Employees

                              join d in _context.Departments
                              on e.DeptId equals d.DeptId into empDept
                              from ed in empDept.DefaultIfEmpty()

                              join a in _context.Attendances
                              on e.EmployeeId equals a.EmployeeId into empAtt
                              from ea in empAtt.DefaultIfEmpty()

                              join s in _context.Salaries
                              on e.EmployeeId equals s.EmployeeId into empSal
                              from es in empSal.DefaultIfEmpty()

                              select new
                              {
                                  e,
                                  ed,
                                  ea,
                                  es
                              }).ToListAsync();

            var employeeDetails = data
                .GroupBy(x => x.e.EmployeeId)
                .Select(g => new DetailedEmployeeViewModel
                {
                    EmployeeId = g.Key,
                    Name = g.First().e.Name,
                    Phone = g.First().e.Phone,
                    Email = g.First().e.Email,
                    JoiningDate = g.First().e.JoiningDate,
                    IsActive = g.First().e.IsActive,

                    // Department (single)
                    EmployeeDepartment = g.First().ed == null ? null : new EmployeeDepartment
                    {
                        DeptId = g.First().ed.DeptId,
                        DeptName = g.First().ed.DeptName,
                        IsActive = g.First().ed.IsActive
                    },

                    // Attendance (list)
                    EmployeeAttendances = g
                        .Where(x => x.ea != null)
                        .Select(x => new EmployeeAttendance
                        {
                            AttendanceId = x.ea.AttendanceId,
                            Date = x.ea.Date,
                            Status = x.ea.Status
                        })
                        .Distinct()
                        .ToList(),

                    // Salary (list)
                    EmployeeSalaries = g
                        .Where(x => x.es != null)
                        .GroupBy(x => new {x.es.Month, x.es.Year})
                        .Select(x => new EmployeeSalary
                        {
                            SalaryId = x.First().es.SalaryId,
                            Month = x.Key.Month,
                            Year = x.Key.Year,
                            Basic = x.Sum(x => x.es.Basic),
                            HRA = x.Sum(x => x.es.HRA),
                            Bonus = x.Sum(x => x.es.Bonus),
                            Deduction = x.Sum(x => x.es.Deduction),
                            NetSalary = x.Sum(x => x.es.NetSalary),
                            Status = x.First().es.Status??"Pending"
                        })
                        .Distinct()
                        .ToList()
                })
                .ToList();

            return View(employeeDetails);
        }
    }
}
