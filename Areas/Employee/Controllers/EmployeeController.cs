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

        private string GenerateEmpCode(string empName)
        {
            if (string.IsNullOrWhiteSpace(empName))
                throw new ArgumentException("Employee name cannot be empty");

            // Split name into words
            var words = empName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            string prefix;

            if (words.Length >= 2)
            {
                // Take first letter of first 2 words
                prefix = $"{char.ToUpper(words[0][0])}{char.ToUpper(words[1][0])}";
            }
            else
            {
                // Only one word → take first letter
                prefix = char.ToUpper(words[0][0]).ToString();
            }

            // Fetch last ID from DB based on prefix
            var lastId = _context.Employees
                .Where(e => e.EmpCode.StartsWith(prefix))
                .OrderByDescending(e => e.EmpCode)
                .Select(e => e.EmpCode)
                .FirstOrDefault();

            int nextNumber = 1;

            if (!string.IsNullOrEmpty(lastId))
            {
                // Extract numeric part
                var numberPart = lastId.Substring(prefix.Length);
                nextNumber = int.Parse(numberPart) + 1;
            }

            // Padding logic
            string formattedNumber;

            if (prefix.Length == 2)
            {
                formattedNumber = nextNumber.ToString("D4"); // AD0001
            }
            else
            {
                formattedNumber = nextNumber.ToString("D5"); // A00001
            }

            return $"{prefix}{formattedNumber}";
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
                                             EmpCode = e.EmpCode,
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
            string empCode = GenerateEmpCode(model.Name ?? "No Name");

            _context.Employees.Add(new Models.Employee
            {
                EmployeeId = employeeId,
                EmpCode = empCode,
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
        public async Task<IActionResult> ActiveToggle(Guid id)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null)
                return NotFound();

            employee.IsActive = !employee.IsActive;

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

            _context.Employees.Remove(employee);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
