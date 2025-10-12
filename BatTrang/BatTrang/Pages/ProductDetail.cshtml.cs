using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BatTrang.Models;

namespace BatTrang.Pages
{
    public class ProductDetailModel : PageModel
    {
        private readonly ExeContext _db;

        public ProductDetailModel(ExeContext db)
        {
            _db = db;
        }

        public Product? Product { get; set; }
        public IList<Product> RelatedProducts { get; set; } = new List<Product>();

        public async Task<IActionResult> OnGetAsync(long id)
        {
            Console.WriteLine($"ProductDetail OnGetAsync called with id: {id}");
            
            Product = await _db.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (Product == null)
            {
                Console.WriteLine($"Product with id {id} not found");
                return NotFound();
            }
            
            Console.WriteLine($"Found product: {Product.Name}, ID: {Product.Id}");

            // Lấy sản phẩm liên quan (cùng danh mục, trừ sản phẩm hiện tại)
            RelatedProducts = await _db.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .Where(p => p.CategoryId == Product.CategoryId && p.Id != Product.Id)
                .OrderByDescending(p => p.Id)
                .Take(4)
                .ToListAsync();

            return Page();
        }
    }
}
