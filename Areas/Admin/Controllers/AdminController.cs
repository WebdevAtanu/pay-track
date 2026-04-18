using Microsoft.AspNetCore.Mvc;
using payroll_mvc.Areas.Admin.Models;
using payroll_mvc.Data;

namespace payroll_mvc.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminController : Controller
    {
        private readonly AppDBContext _context;

        public AdminController(AppDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var admins = _context.Admins.ToList();
            var adminViewModels = admins.Select(a => new AdminViewModel
            {
                AdminId = a.AdminId,
                Name = a.Name,
                Phone = a.Phone,
                Email = a.Email,
                Password = a.Password
            }).ToList();
            return View(adminViewModels);
        }
    }
}
