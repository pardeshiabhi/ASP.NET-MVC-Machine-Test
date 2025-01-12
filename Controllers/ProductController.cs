using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductDetail.API.Data;
using ProductDetail.API.Models;
using System.Linq;

namespace ProductDetail.API.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int pageNo = 1)
        {
            int pageSize = 10;
            var totalProducts = await _context.Products.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

            var products = await _context.Products
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .Include(p => p.Category)
                .ToListAsync();

            ViewData["CurrentPage"] = pageNo;
            ViewData["TotalPages"] = totalPages;

            return View(products);
        }


        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (!_context.Categories.Any(c => c.CategoryId == product.CategoryId))
            {
                return BadRequest("Category does not exist.");
            }

            if (_context.Products.Any(p => p.ProductName == product.ProductName))
            {
                ModelState.AddModelError("ProductName", "Product already exists.");
                ViewBag.Categories = await _context.Categories.ToListAsync();
                return View(product); 
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Product added successfully!";
            return RedirectToAction(nameof(Index));
        }





        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.ProductId) return NotFound();

            if (_context.Categories.Any(c => c.CategoryName == product.Category.CategoryName && c.CategoryId != product.CategoryId))
            {
                return BadRequest("Category with this name already exists.");
            }

            try
            {
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.ProductId)) return NotFound();
                throw;
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(product);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return NotFound();

            return View(product);
        }

        
       

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int? ProductId)
        {
            var product = await _context.Products.FindAsync(ProductId);

            if (product == null)
            {
                
                TempData["ErrorMessage"] = "The product you are trying to delete does not exist.";
                return RedirectToAction(nameof(Index));
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Product deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(p => p.ProductId == id);
        }
    }
}
