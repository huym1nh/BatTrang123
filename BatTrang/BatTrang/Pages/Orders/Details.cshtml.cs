using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BatTrang.Models;

namespace BatTrang.Pages.Orders
{
    public class DetailsModel : PageModel
    {
        private readonly ExeContext _db;
        public DetailsModel(ExeContext db) { _db = db; }

        public Order Order { get; set; } = default!;
        public List<OrderItem> Items { get; set; } = new();

        // Thông tin shop (có thể lấy từ cấu hình)
        public string ShopName { get; set; } = "Gốm Bát Tràng";
        public string ShopAddress { get; set; } = "Số 1 Gốm, Gia Lâm, Hà Nội";
        public string ShopPhone { get; set; } = "0123 456 789";
        public string ShopEmail { get; set; } = "support@battrang.vn";

        public async Task<IActionResult> OnGetAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return RedirectToPage("/Index");

            // lấy order + items (AsNoTracking cho nhanh)
            var order = await _db.Orders.AsNoTracking()
                .FirstOrDefaultAsync(o => o.OrderCode == code);

            if (order == null)
                return RedirectToPage("/Index");

            var items = await _db.OrderItems.AsNoTracking()
                .Where(i => i.OrderId == order.Id)
                .OrderBy(i => i.Id)
                .ToListAsync();

            Order = order;
            Items = items;

            return Page();
        }

        public string FormatCurrency(decimal value)
            => string.Format(new CultureInfo("vi-VN"), "{0:C0}", value);
    }
}
