using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var summary = await _context.TaskItems
                .Where(t => t.UserId == userId)
                .GroupBy(t => t.Status)
                .Select(g => new StatusSummary { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            return View(summary);
        }
    }

    public class StatusSummary
    {
        public string Status { get; set; }
        public int Count { get; set; }
    }
}
