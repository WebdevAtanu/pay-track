using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using payroll_mvc.Areas.Admin.Models;
using payroll_mvc.Data;
using payroll_mvc.Models;
using payroll_mvc.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace payroll_mvc.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class AttendanceController : Controller
    {
        private readonly AppDBContext _context;

        public AttendanceController(AppDBContext context)
        {
            _context = context;
        }

        private async Task<List<AttendanceViewModel>> GetEmployeeAttendance(DateTime date)
        {
            var attendanceDetails = await (
                from e in _context.Employees
                where e.IsActive == true

                from eat in _context.Attendances
                    .Where(a => a.EmployeeId == e.EmployeeId &&
                                a.Date >= date.Date &&
                                a.Date < date.Date.AddDays(1))
                    .DefaultIfEmpty()

                select new AttendanceViewModel
                {
                    EmployeeId = e.EmployeeId,
                    EmpCode = e.EmpCode,
                    Name = e.Name,
                    Date = eat != null ? eat.Date : date,
                    Status = eat != null ? eat.Status : "Absent",
                    Note = eat != null ? eat.Note : ""
                }
            ).ToListAsync();

            return attendanceDetails;
        }

        public async Task<IActionResult> Index(DateTime? today)
        {
            var selectedDate = today ?? DateTime.Today;
            var attendanceDetails = await GetEmployeeAttendance(selectedDate);
            return View(attendanceDetails);
        }

        [HttpPost]
        public async Task<IActionResult> SaveAttendance(List<AttendanceViewModel> model, DateTime date)
        {
            foreach (var item in model)
            {
                var existing = await _context.Attendances
                    .FirstOrDefaultAsync(x =>
                        x.EmployeeId == item.EmployeeId &&
                        x.Date >= date.Date &&
                        x.Date < date.Date.AddDays(1));

                if (existing != null)
                {
                    existing.Status = item.Status;
                    existing.Note = item.Note;
                }
                else
                {
                    _context.Attendances.Add(new Attendance
                    {
                        EmployeeId = item.EmployeeId,
                        Date = date,
                        Status = item.Status,
                        Note = item.Note
                    });
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { today = date });
        }
    }
}
