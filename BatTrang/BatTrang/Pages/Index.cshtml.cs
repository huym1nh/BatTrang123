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
        public IList<Product> BestSellers { get; set; } = new List<Product>();

        public async Task OnGet()
        {
            FeaturedProducts = await _db.Products
                .AsNoTracking()
                .OrderByDescending(p => p.Id)
                .Take(6)
                .ToListAsync();

            // Best sellers calculated from order items (by quantity)
            var topIdsWithQty = await _db.OrderItems
                .AsNoTracking()
                .GroupBy(oi => oi.ProductId)
                .Select(g => new { ProductId = g.Key, Qty = g.Sum(x => x.Quantity) })
                .OrderByDescending(x => x.Qty)
                .Take(8)
                .ToListAsync();

            if (topIdsWithQty.Count > 0)
            {
                var ids = topIdsWithQty.Select(x => x.ProductId).ToList();
                var qtyMap = topIdsWithQty.ToDictionary(x => x.ProductId, x => x.Qty);

                var products = await _db.Products
                    .AsNoTracking()
                    .Where(p => ids.Contains(p.Id))
                    .ToListAsync();

                BestSellers = products
                    .OrderByDescending(p => qtyMap.TryGetValue(p.Id, out var q) ? q : 0)
                    .ToList();
            }
            else
            {
                // Fallback: latest products
                BestSellers = await _db.Products
                    .AsNoTracking()
                    .OrderByDescending(p => p.Id)
                    .Take(8)
                    .ToListAsync();
            }
        }
    }
}
