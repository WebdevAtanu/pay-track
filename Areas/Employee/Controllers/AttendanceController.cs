using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenCvSharp;
using payroll_mvc.Areas.Admin.ViewModels;
using payroll_mvc.Controllers;
using payroll_mvc.Data;
using payroll_mvc.Entities;
using payroll_mvc.ViewModels;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace payroll_mvc.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class AttendanceController : BaseController
    {
        private readonly AppDBContext _context;

        public AttendanceController(AppDBContext context)
        {
            _context = context;
        }

        private Guid? GetCurrentEmployeeId()
        {
            var employeeIdClaim = User.FindFirst("EmployeeId")?.Value;
            if (string.IsNullOrEmpty(employeeIdClaim) || !Guid.TryParse(employeeIdClaim, out var employeeId))
            {
                return null;
            }
            return employeeId;
        }

        private async Task<List<AttendanceViewModel>> GetEmployeeAttendance(DateTime startDate, DateTime endDate)
        {
            var currentEmployeeId = GetCurrentEmployeeId();
            if (currentEmployeeId == null)
            {
                return new List<AttendanceViewModel>();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == currentEmployeeId);

            if (employee == null)
            {
                return new List<AttendanceViewModel>();
            }

            var attendances = await _context.Attendances
                .Where(a => a.EmployeeId == currentEmployeeId &&
                           a.Date.HasValue &&
                           a.Date.Value >= startDate.Date &&
                           a.Date.Value <= endDate.Date.AddDays(1).AddTicks(-1))
                .ToListAsync();

            var attendanceDetails = new List<AttendanceViewModel>();

            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                var attendance = attendances.FirstOrDefault(a =>
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

            return attendanceDetails;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            var selectedStartDate = startDate ?? DateTime.Today;
            var selectedEndDate = endDate ?? DateTime.Today;

            var attendanceDetails = await GetEmployeeAttendance(selectedStartDate, selectedEndDate);
            return View(attendanceDetails);
        }

        public async Task<IActionResult> FaceAttendance(Guid id, DateTime? date)
        {
            var currentEmployeeId = GetCurrentEmployeeId();
            if (currentEmployeeId == null || id != currentEmployeeId)
            {
                return Forbid();
            }

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
            var currentEmployeeId = GetCurrentEmployeeId();
            if (currentEmployeeId == null || attendanceRequest.EmployeeId != currentEmployeeId)
            {
                return Forbid();
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
