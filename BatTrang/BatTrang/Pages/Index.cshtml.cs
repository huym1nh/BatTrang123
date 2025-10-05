using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BatTrang.Models;

namespace BatTrang.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ExeContext _db;

        public IndexModel(ILogger<IndexModel> logger, ExeContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IList<Product> FeaturedProducts { get; set; } = new List<Product>();

        public async Task OnGet()
        {
            FeaturedProducts = await _db.Products
                .AsNoTracking()
                .OrderByDescending(p => p.Id)
                .Take(6)
                .ToListAsync();
        }
    }
}
