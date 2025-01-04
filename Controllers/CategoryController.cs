using Microsoft.AspNetCore.Mvc;
using ProductDetail.API.Data;
using ProductDetail.API.Models;

namespace ProductDetail.API.Controllers
{
    [ApiController]
    [Route("Api/[controller]")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetCategories() => Ok(_context.Categories.ToList());

        [HttpPost]
        public IActionResult AddCategory([FromQuery] Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
            return Ok(category);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCategory(int id, [FromQuery] Category category)
        {
            var existing = _context.Categories.Find(id);
            if (existing == null)
                return NotFound();

            existing.CategoryName = category.CategoryName;
            _context.SaveChanges();
            return Ok(category);
        }

        [HttpDelete]
        public IActionResult DeleteCategory(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
                return NotFound();

            _context.Categories.Remove(category);
            _context.SaveChanges();
            return Ok();
        }
      
    }
}
