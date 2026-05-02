using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using payroll_mvc.Areas.Admin.ViewModels;
using payroll_mvc.Data;
using payroll_mvc.Models;

namespace payroll_mvc.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class DepartmentController : Controller
    {
        private readonly AppDBContext _context;

        public DepartmentController(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var deptDetails = await _context.Departments.Select(d => new DepartmentViewModel
            {
                DeptId = d.DeptId,
                DeptCode = d.DeptCode,
                DeptName = d.DeptName,
                IsActive = d.IsActive,
            }).ToListAsync();

            return View(deptDetails);
        }

        public IActionResult Add()
        {
            return View(new DepartmentViewModel());
        }

        private async Task<bool> IsDeptExist(string deptName)
        {
            var dept = await _context.Departments.SingleOrDefaultAsync(e => e.DeptName.ToLower() == deptName);
            if (dept == null)
            {
                return false;
            }
            return true;
        }

        private string GenerateDeptCode(string deptName)
        {
            // Split name into words
            var words = deptName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

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
            var lastId = _context.Departments
                .Where(e => e.DeptCode.StartsWith(prefix))
                .OrderByDescending(e => e.DeptCode)
                .Select(e => e.DeptCode)
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
                formattedNumber = nextNumber.ToString("D2");
            }
            else
            {
                formattedNumber = nextNumber.ToString("D3");
            }

            return $"{prefix}{formattedNumber}";
        }

        [HttpPost]
        public async Task<IActionResult> Add(DepartmentViewModel model)
        {
            //if (!ModelState.IsValid)
            //    return View(model);

            bool deptExist = await IsDeptExist(model.DeptName.ToLower());
            if (deptExist)
            {
                TempData["ErrorMessage"] = "Department already exists";
                return RedirectToAction("Add");
            }

            Guid deptId = Guid.NewGuid();
            string deptCode = GenerateDeptCode(model.DeptName ?? "No Name");

            _context.Departments.Add(new Department
            {
                DeptId = model.DeptId,
                DeptCode = deptCode,
                DeptName = model.DeptName,
                CreatedAt = DateTime.Now,
                IsActive = true
            });

            await _context.SaveChangesAsync();

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
