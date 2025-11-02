using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BatTrang.Models;

namespace BatTrang.Pages
{
    public class ProductsModel : PageModel
    {
        private readonly ExeContext _db;

        public ProductsModel(ExeContext db)
        {
            _db = db;
        }

        public IList<Product> Products { get; set; } = new List<Product>();
        public IList<Category> Categories { get; set; } = new List<Category>();
        public Dictionary<long, int> CategoryProductCounts { get; set; } = new Dictionary<long, int>();
        
        [BindProperty(SupportsGet = true)]
        public long? CategoryId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SortBy { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        public async Task OnGetAsync()
        {
            // Lấy danh sách categories
            Categories = await _db.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync();

            // Đếm số sản phẩm cho mỗi category
            var allProducts = await _db.Products
                .AsNoTracking()
                .ToListAsync();

            CategoryProductCounts = allProducts
                .GroupBy(p => p.CategoryId)
                .ToDictionary(g => g.Key, g => g.Count());

            // Lấy danh sách products với filter
            var query = _db.Products
                .Include(p => p.Category)
                .AsNoTracking();

            // Filter by category
            if (CategoryId.HasValue && CategoryId > 0)
            {
                query = query.Where(p => p.CategoryId == CategoryId);
            }

            // Filter by search
            if (!string.IsNullOrWhiteSpace(Search))
            {
                var searchLower = Search.ToLower();
                query = query.Where(p => 
                    p.Name.ToLower().Contains(searchLower) ||
                    (p.Description != null && p.Description.ToLower().Contains(searchLower))
                );
            }

            // Apply sorting
            query = SortBy switch
            {
                "price-asc" => query.OrderBy(p => p.Price),
                "price-desc" => query.OrderByDescending(p => p.Price),
                "name-asc" => query.OrderBy(p => p.Name),
                "name-desc" => query.OrderByDescending(p => p.Name),
                _ => query.OrderByDescending(p => p.Id) // newest first (default)
            };

            Products = await query.ToListAsync();
        }
    }
}
