using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductDetail.API.Data;
using ProductDetail.API.Models;

namespace ProductDetail.API.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int pageNo = 1)
        {
            int pageSize = 10;
            var totalCategories = await _context.Categories.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCategories / pageSize);

            var categories = await _context.Categories
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["CurrentPage"] = pageNo;
            ViewData["TotalPages"] = totalPages;

            return View(categories);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (_context.Categories.Any(c => c.CategoryName == category.CategoryName))
            {
                ModelState.AddModelError("CategoryName", "Category already exists.");
                ViewBag.Categories = await _context.Categories.ToListAsync();
                return View(category); 
            }

            // Add the product to the database
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Category added successfully!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.CategoryId) return NotFound();

            try
            {
                _context.Categories.Update(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(category.CategoryId)) return NotFound();
                throw;
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null) return NotFound();

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int? CategoryId)
        {
            var category = await _context.Categories.FindAsync(CategoryId);

            if (category == null)
            {

                TempData["ErrorMessage"] = "The cateory you are trying to delete does not exist.";
                return RedirectToAction(nameof(Index));
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "category deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(c => c.CategoryId == id);
        }
    }
}



