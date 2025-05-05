using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;
using Microsoft.AspNetCore.Identity;

namespace TaskManager.Controllers
{
    public class TaskItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TaskItemsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var userTasks = _context.TaskItems
                .Include(t => t.User)
                .Where(t => t.UserId == userId);

            return View(await userTasks.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var taskItem = await _context.TaskItems
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (taskItem == null) return NotFound();

            return View(taskItem);
        }

        public IActionResult Create()
        {
            var viewModel = new TaskItemFormViewModel
            {
                Categories = new MultiSelectList(_context.Categories, "Id", "Name")
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskItemFormViewModel model)
        {
            ModelState.Remove("TaskItem.UserId");
            ModelState.Remove("TaskItem.User");
            ModelState.Remove("TaskItem.TaskCategories");
            ModelState.Remove("SelectedCategoryIds");
            ModelState.Remove("Categories");

            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                model.TaskItem.UserId = userId;
                model.TaskItem.TaskCategories = model.SelectedCategoryIds
                    .Select(catId => new TaskCategory { CategoryId = catId })
                    .ToList();

                _context.TaskItems.Add(model.TaskItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            model.Categories = new MultiSelectList(_context.Categories, "Id", "Name", model.SelectedCategoryIds);
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var taskItem = await _context.TaskItems
                .Include(t => t.TaskCategories)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (taskItem == null) return NotFound();

            var viewModel = new TaskItemFormViewModel
            {
                TaskItem = taskItem,
                SelectedCategoryIds = taskItem.TaskCategories.Select(tc => tc.CategoryId).ToList(),
                Categories = new MultiSelectList(_context.Categories, "Id", "Name")
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaskItemFormViewModel model)
        {
            ModelState.Remove("TaskItem.UserId");
            ModelState.Remove("TaskItem.User");
            ModelState.Remove("TaskItem.TaskCategories");
            ModelState.Remove("SelectedCategoryIds");
            ModelState.Remove("Categories");

            if (id != model.TaskItem.Id)
                return NotFound();

            var userId = _userManager.GetUserId(User);
            var taskItem = await _context.TaskItems
                .Include(t => t.TaskCategories)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (taskItem == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    taskItem.Title = model.TaskItem.Title;
                    taskItem.Description = model.TaskItem.Description;
                    taskItem.DueDate = model.TaskItem.DueDate;
                    taskItem.Status = model.TaskItem.Status;
                    taskItem.Priority = model.TaskItem.Priority;

                    taskItem.TaskCategories.Clear();
                    taskItem.TaskCategories = model.SelectedCategoryIds
                        .Select(id => new TaskCategory { CategoryId = id, TaskItemId = taskItem.Id })
                        .ToList();

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskItemExists(model.TaskItem.Id)) return NotFound();
                    else throw;
                }
            }

            model.Categories = new MultiSelectList(_context.Categories, "Id", "Name", model.SelectedCategoryIds);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkComplete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var task = await _context.TaskItems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (task == null) return NotFound();

            task.Status = "Completed";
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var taskItem = await _context.TaskItems
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (taskItem == null) return NotFound();

            return View(taskItem);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);
            var taskItem = await _context.TaskItems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (taskItem != null)
            {
                _context.TaskItems.Remove(taskItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TaskItemExists(int id)
        {
            return _context.TaskItems.Any(e => e.Id == id);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus([FromBody] int id)
        {
            var userId = _userManager.GetUserId(User);
            var task = await _context.TaskItems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (task == null) return NotFound();

            task.Status = task.Status == "Completed" ? "Pending" : "Completed";
            await _context.SaveChangesAsync();

            return Json(new { success = true, status = task.Status });
        }

    }
}