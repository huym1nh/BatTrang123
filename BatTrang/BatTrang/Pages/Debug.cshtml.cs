using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BatTrang.Models;

namespace BatTrang.Pages
{
    public class DebugModel : PageModel
    {
        private readonly ExeContext _db;

        public DebugModel(ExeContext db)
        {
            _db = db;
        }

        public List<Product> Products { get; set; } = new List<Product>();
        public List<Category> Categories { get; set; } = new List<Category>();

        public async Task OnGetAsync()
        {
            try
            {
                Products = await _db.Products
                    .Include(p => p.Category)
                    .ToListAsync();

                Categories = await _db.Categories.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
        }
    }
}
