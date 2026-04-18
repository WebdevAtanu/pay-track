using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using payroll_mvc.Areas.Admin.Models;
using payroll_mvc.Data;
using payroll_mvc.ViewModels;

namespace payroll_mvc.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class EmployeeController : Controller
    {
        private readonly AppDBContext _context;

        public EmployeeController(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var employeeDetails = await (from e in _context.Employees
                                   join d in _context.Departments
                                      on e.DeptId equals d.DeptId into empDept
                                   from ed in empDept.DefaultIfEmpty()

                                   join s in _context.Salaries
                                     on e.EmployeeId equals s.EmployeeId into empSal
                                   from es in empSal.DefaultIfEmpty()

                                   select new EmployeeViewModel
                                   {
                                       EmployeeId = e.EmployeeId,
                                       Name = e.Name,
                                       Phone = e.Phone,
                                       Email = e.Email,
                                       DeptId = ed != null ? ed.DeptId : (Guid?)null,
                                       DepartmentName = ed != null ? ed.DeptName : null,
                                       JoiningDate = e.JoiningDate,
                                       BasicSalary = es.Basic
                                   }).ToListAsync();
            return View(employeeDetails);
        }
    }
}
