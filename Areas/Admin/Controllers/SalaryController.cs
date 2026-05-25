using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using payroll_mvc.Areas.Admin.ViewModels;
using payroll_mvc.Controllers;
using payroll_mvc.Data;
using payroll_mvc.Entities;

namespace payroll_mvc.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SalaryController : BaseController
    {
        private readonly AppDBContext _context;

        public SalaryController(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(Guid employeeId)
        {
            var empDetails = await _context.Employees
                .Where(e => e.EmployeeId == employeeId)
                .Select(e => new { e.EmployeeId, e.Name })
                .FirstOrDefaultAsync();

            var salaryDetails = await _context.Salaries
                .Where(s => s.EmployeeId == employeeId)
                .Select(s => new SalaryViewModel
                {
                    SalaryId = s.SalaryId,
                    EmployeeId = s.EmployeeId,
                    EmployeeName = empDetails.Name,
                    Month = s.Month,
                    Year = s.Year,
                    Basic = s.Basic,
                    HRA = s.HRA,
                    Bonus = s.Bonus,
                    Deduction = s.Deduction,
                    NetSalary = s.NetSalary,
                    Status = s.Status
                }).ToListAsync();

            return View(salaryDetails);
        }

        public IActionResult Add()
        {
            return View(new Salary()); // Return empty model for form binding
        }

        [HttpPost]
        public async Task<IActionResult> Add(DepartmentViewModel model)
        {
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var data = await _context.Departments.Where(d => d.DeptId == id)
                .Select(d => new DepartmentViewModel
                {
                    DeptId = d.DeptId,
                    DeptCode = d.DeptCode,
                    DeptName = d.DeptName,
                }).FirstOrDefaultAsync();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DepartmentViewModel model)
        {
            var dept = await _context.Departments.FirstOrDefaultAsync(e => e.DeptId == model.DeptId);
            if (dept == null)
                return NotFound();

            dept.DeptName = model.DeptName;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ActiveToggle(Guid id)
        {
            var dept = await _context.Departments
                .FirstOrDefaultAsync(e => e.DeptId == id);

            if (dept == null)
                return NotFound();

            dept.IsActive = !dept.IsActive;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var dept = await _context.Departments
                .FirstOrDefaultAsync(e => e.DeptId == id);

            if (dept == null)
                return NotFound();

            _context.Departments.Remove(dept);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
