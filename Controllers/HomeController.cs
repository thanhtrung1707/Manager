using Manager.Data;
using Manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchName, string searchGrId, int pageNumber = 1)
        {
            int pageSize = 5;
            var query = _context.Staff.AsQueryable();

            if (!string.IsNullOrEmpty(searchName))
            {
                query = query.Where(s => s.Name.StartsWith(searchName));
            }

            if (!string.IsNullOrEmpty(searchGrId))
            {
                query = query.Where(s => s.GrId.Contains(searchGrId));
            }

            var entities = await query.OrderBy(s => s.GrId)
                                      .ThenBy(s => s.Id)
                                      .Skip((pageNumber - 1) * pageSize)
                                      .Take(pageSize)
                                      .ToListAsync();

            var totalRecords = await query.CountAsync();
            var viewModel = new StaffViewModel
            {
                StaffList = entities,
                PageNumber = pageNumber,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize)
            };

            return View(viewModel);
        }

        public IActionResult AddStaff()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddStaff(Staff staff)
        {
            if (ModelState.IsValid)
            {
                _context.Add(staff);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Staff added successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(staff);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            var staff = await _context.Staff.FindAsync(id);
            if (staff == null)
            {
                return NotFound();
            }

            _context.Staff.Remove(staff);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Staff deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> EditStaff(int id)
        {
            var staff = await _context.Staff.FindAsync(id);

            if (staff == null)
            {
                return NotFound();
            }

            return View(staff);
        }

        [HttpPost]
        public async Task<IActionResult> EditStaff(int id, Staff staff)
        {
            if (id != staff.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(staff);
                    await _context.SaveChangesAsync();

                    TempData["Message"] = "Staff updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StaffExists(staff.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(staff);
        }

        private bool StaffExists(int id)
        {
            return _context.Staff.Any(e => e.Id == id);
        }
    }

    public class StaffViewModel
    {
        public List<Staff> StaffList { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
    }
}
