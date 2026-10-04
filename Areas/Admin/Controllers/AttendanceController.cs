using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using payroll_mvc.Areas.Admin.ViewModels;
using payroll_mvc.Areas.Employee.ViewModels;
using payroll_mvc.Controllers;
using payroll_mvc.Data;
using payroll_mvc.Entities;

namespace payroll_mvc.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AttendanceController : BaseController
    {
        private readonly AppDBContext _context;

        public AttendanceController(AppDBContext context)
        {
            _context = context;
        }

        private async Task<List<AttendanceViewModel>> GetAllEmployeesAttendance(DateTime startDate, DateTime endDate)
        {
            var employees = await _context.Employees.ToListAsync();
            var attendances = await _context.Attendances
                .Where(a => a.Date.HasValue &&
                           a.Date.Value >= startDate.Date &&
                           a.Date.Value <= endDate.Date.AddDays(1).AddTicks(-1))
                .ToListAsync();

            var attendanceDetails = new List<AttendanceViewModel>();

            foreach (var employee in employees)
            {
                for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
                {
                    var attendance = attendances.FirstOrDefault(a =>
                        a.EmployeeId == employee.EmployeeId &&
                        a.Date.HasValue &&
                        a.Date.Value.Date == date);

                    attendanceDetails.Add(new AttendanceViewModel
                    {
                        EmployeeId = employee.EmployeeId,
                        EmpCode = employee.EmpCode,
                        Name = employee.Name,
                        Date = date,
                        Status = attendance != null ? attendance.Status : "Absent",
                        Note = attendance != null ? attendance.Note : ""
                    });
                }
            }

            return attendanceDetails;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            var selectedStartDate = startDate ?? DateTime.Today;
            var selectedEndDate = endDate ?? DateTime.Today;

            var attendanceDetails = await GetAllEmployeesAttendance(selectedStartDate, selectedEndDate);
            var statusOptions = await _context.Statuses
            .Where(s => s.IsActive == true)
            .Select(s => new StatusViewModel
            {
                StatusId = s.StatusId,
                StatusName = s.StatusName,
                IsActive = s.IsActive ?? true,
                CreatedAt = s.CreatedAt ?? DateTime.Now
            })
            .ToListAsync();

            ViewData["StatusOptions"] = statusOptions;
            return View(attendanceDetails);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAttendance(List<AttendanceViewModel> model, DateTime startDate, DateTime endDate)
        {
            if (model == null || !model.Any())
            {
                return RedirectToAction("Index", new { startDate = startDate.ToString("yyyy-MM-dd"), endDate = endDate.ToString("yyyy-MM-dd") });
            }

            foreach (var item in model)
            {
                if (item == null) continue;

                var existing = await _context.Attendances
                    .FirstOrDefaultAsync(x =>
                        x.EmployeeId == item.EmployeeId &&
                        x.Date.HasValue &&
                        x.Date.Value.Date == item.Date.Value.Date);

                if (existing != null)
                {
                    existing.Status = item.Status;
                    existing.Note = item.Note;
                }
                else if (item.Date.HasValue)
                {
                    _context.Attendances.Add(new Attendance
                    {
                        AttendanceId = Guid.NewGuid(),
                        EmployeeId = item.EmployeeId,
                        Date = item.Date.Value,
                        Status = item.Status,
                        Note = item.Note
                    });
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { startDate = startDate.ToString("yyyy-MM-dd"), endDate = endDate.ToString("yyyy-MM-dd") });
        }

        public async Task<IActionResult> FaceAttendance(Guid id, DateTime? date)
        {
            var empData = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == id);
            var selectedDate = date ?? DateTime.Today;

            var model = new EmpFaceData
            {
                EmployeeId = id,
                EmpCode = empData?.EmpCode ?? "",
                Name = empData?.Name,
                Phone = empData?.Phone,
                Email = empData?.Email,
                Descriptor = empData?.FaceDescriptor ?? "",
                SelectedDate = selectedDate
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult MarkAttendance([FromBody] AttendanceRequest attendanceRequest)
        {
            // Check if attendance already exists for this date
            var existingAttendance = _context.Attendances
                .FirstOrDefault(a => a.EmployeeId == attendanceRequest.EmployeeId &&
                                   a.Date.HasValue &&
                                   a.Date.Value.Date == attendanceRequest.Date.Date);

            if (existingAttendance != null)
            {
                return BadRequest("Attendance already marked for this date");
            }

            Attendance attendance = new Attendance()
            {
                AttendanceId = Guid.NewGuid(),
                EmployeeId = attendanceRequest.EmployeeId,
                Date = attendanceRequest.Date,
                Status = "Present"
            };

            _context.Attendances.Add(attendance);

            _context.SaveChanges();

            return Ok();
        }
    }
}
