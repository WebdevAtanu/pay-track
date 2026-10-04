using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using payroll_mvc.Areas.Admin.ViewModels;
using payroll_mvc.Controllers;
using payroll_mvc.Data;
using payroll_mvc.Entities;

namespace payroll_mvc.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StatusController : BaseController
    {
        private readonly AppDBContext _context;

        public StatusController(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var statusDetails = await _context.Statuses
                .Select(s => new StatusViewModel
                {
                    StatusId = s.StatusId,
                    StatusName = s.StatusName,
                    IsActive = s.IsActive ?? true,
                    CreatedAt = s.CreatedAt ?? DateTime.Now
                })
                .ToListAsync();

            return View(statusDetails);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View(new StatusViewModel
            {
                IsActive = true
            });
        }

        [HttpPost]
        public async Task<IActionResult> Add(StatusViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var statusExists = await _context.Statuses
                .AnyAsync(s => s.StatusName == model.StatusName);

            if (statusExists)
            {
                TempData["ErrorMessage"] = "Status name already exists.";
                return View(model);
            }

            var status = new Status
            {
                StatusId = Guid.NewGuid(),
                StatusName = model.StatusName,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.Statuses.Add(status);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var status = await _context.Statuses
                .FirstOrDefaultAsync(s => s.StatusId == id);

            if (status == null)
            {
                return NotFound();
            }

            var model = new StatusViewModel
            {
                StatusId = status.StatusId,
                StatusName = status.StatusName,
                IsActive = status.IsActive ?? true,
                CreatedAt = status.CreatedAt ?? DateTime.Now
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(StatusViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var status = await _context.Statuses
                .FirstOrDefaultAsync(s => s.StatusId == model.StatusId);

            if (status == null)
            {
                return NotFound();
            }

            var duplicateStatus = await _context.Statuses
                .AnyAsync(s =>
                    s.StatusId != model.StatusId &&
                    s.StatusName == model.StatusName);

            if (duplicateStatus)
            {
                TempData["ErrorMessage"] = "Status name already exists.";
                return View(model);
            }

            status.StatusName = model.StatusName;
            status.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActiveToggle(Guid id)
        {
            var status = await _context.Statuses
                .FirstOrDefaultAsync(s => s.StatusId == id);

            if (status == null)
            {
                return NotFound();
            }

            status.IsActive = !(status.IsActive ?? true);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var status = await _context.Statuses
                .FirstOrDefaultAsync(s => s.StatusId == id);

            if (status == null)
            {
                return NotFound();
            }

            _context.Statuses.Remove(status);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
