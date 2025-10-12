using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BatTrang.Models;

namespace BatTrang.Pages
{
    public class OrderSuccessModel : PageModel
    {
        private readonly ExeContext _db;
        public OrderSuccessModel(ExeContext db) { _db = db; }

        public string OrderCode { get; set; } = "";
        public string PaymentMethod { get; set; } = "COD";

        public async Task<IActionResult> OnGetAsync(string code)
        {
            var order = await _db.Orders.AsNoTracking()
                .FirstOrDefaultAsync(o => o.OrderCode == code);
            if (order == null) return RedirectToPage("/Index");

            OrderCode = order.OrderCode;
            PaymentMethod = order.PaymentMethod;
            return Page();
        }
    }
}
