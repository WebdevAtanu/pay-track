using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using payroll_mvc.Areas.Admin.ViewModels;
using payroll_mvc.Controllers;
using payroll_mvc.Data;
using payroll_mvc.Entities;
using payroll_mvc.ViewModels;
using BCrypt.Net;

namespace payroll_mvc.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class EmployeeDashboardController : BaseController
    {
        private readonly AppDBContext _context;

        public EmployeeDashboardController(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var employeeDetails = await (from e in _context.Employees
                                         join d in _context.Departments
                                            on e.DeptId equals d.DeptId into empDept
                                         from ed in empDept.DefaultIfEmpty()

                                         select new EmployeeViewModel
                                         {
                                             EmployeeId = e.EmployeeId,
                                             EmpCode = e.EmpCode,
                                             Name = e.Name,
                                             Phone = e.Phone,
                                             Email = e.Email,
                                             DeptId = ed != null ? ed.DeptId : (Guid?)null,
                                             DepartmentName = ed != null ? ed.DeptName : null,
                                             JoiningDate = e.JoiningDate,
                                             FaceDescriptor = e.FaceDescriptor,
                                             IsActive = e.IsActive
                                         }).OrderBy(e => e.Name).ToListAsync();
            return View(employeeDetails);
        }
    }
}
