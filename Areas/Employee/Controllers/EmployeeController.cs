using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using payroll_mvc.Areas.Admin.Models;
using payroll_mvc.Data;
using payroll_mvc.Models;
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
                                             BasicSalary = es.Basic,
                                             IsActive = e.IsActive
                                         }).ToListAsync();
            return View(employeeDetails);
        }

        public IActionResult Add()
        {
            return View(new EmployeeViewModel());
        }

        private async Task<bool> IsEmailExist(string email)
        {
            var employee = await _context.Employees.SingleOrDefaultAsync(e => e.Email == email);
            if (employee == null)
            {
                return false;
            }
            return true;
        }

        [HttpPost]
        public async Task<IActionResult> Add(EmployeeViewModel model)
        {
            //if (!ModelState.IsValid)
            //    return View(model);

            bool emailExist = await IsEmailExist(model.Email);
            if (emailExist)
            {
                TempData["ErrorMessage"] = "Email already exists";
                return RedirectToAction("Add");
            }

            Guid employeeId = Guid.NewGuid();

            _context.Employees.Add(new Models.Employee
            {
                EmployeeId = employeeId,
                Name = model.Name,
                Phone = model.Phone,
                Email = model.Email,
                DeptId = model.DeptId,
                JoiningDate = model.JoiningDate,
                IsActive = true
            });

            // create salary if not exists
            _context.Salaries.Add(new Salary
            {
                EmployeeId = employeeId,
                Basic = model.BasicSalary ?? 0
            });


            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var data = await (from e in _context.Employees
                              join d in _context.Departments
                                 on e.DeptId equals d.DeptId into empDept
                              from ed in empDept.DefaultIfEmpty()

                              join s in _context.Salaries
                                on e.EmployeeId equals s.EmployeeId into empSal
                              from es in empSal.DefaultIfEmpty()

                              where e.EmployeeId == id

                              select new EmployeeViewModel
                              {
                                  EmployeeId = e.EmployeeId,
                                  Name = e.Name,
                                  Phone = e.Phone,
                                  Email = e.Email,
                                  DeptId = ed != null ? ed.DeptId : (Guid?)null,
                                  DepartmentName = ed != null ? ed.DeptName : null,
                                  JoiningDate = e.JoiningDate,
                                  BasicSalary = es.Basic,
                                  IsActive = e.IsActive
                              }).FirstOrDefaultAsync();
            return View(data);

        }

        [HttpPost]
        public async Task<IActionResult> Edit(EmployeeViewModel model)
        {
            //if (!ModelState.IsValid)
            //    return View(model);

            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == model.EmployeeId);
            if (employee == null)
                return NotFound();

            var salary = await _context.Salaries
                .FirstOrDefaultAsync(s => s.EmployeeId == employee.EmployeeId);

            // Update employee
            employee.Name = model.Name;
            employee.Phone = model.Phone;
            employee.Email = model.Email;
            //employee.DeptId = model.DeptId;
            employee.IsActive = model.IsActive;

            // Update salary correctly
            if (salary != null)
            {
                salary.Basic = model.BasicSalary ?? 0;
            }
            else
            {
                // create salary if not exists
                _context.Salaries.Add(new Salary
                {
                    EmployeeId = employee.EmployeeId,
                    Basic = model.BasicSalary ?? 0
                });
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null)
                return NotFound();

            employee.IsActive = false;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
