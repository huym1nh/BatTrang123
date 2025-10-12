using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using BatTrang.Models;

namespace BatTrang.Pages
{
    public class CheckoutModel : PageModel
    {
        private readonly ExeContext _db;
        private const string CartSessionKey = "Cart";

        public CheckoutModel(ExeContext db) { _db = db; }

        [BindProperty] public InputModel Input { get; set; } = new();
        public SummaryModel Summary { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var (items, subtotal) = await ReadCartAndRecalcAsync();
            if (items.Count == 0) return RedirectToPage("/Cart");

            Summary = new SummaryModel { Subtotal = subtotal, ShippingFee = 30000, Discount = 0 };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var (items, subtotal) = await ReadCartAndRecalcAsync();
            if (items.Count == 0) return RedirectToPage("/Cart");

            var shipping = 30000m; var discount = 0m; var total = subtotal + shipping - discount;
            var code = GenerateOrderCode();

            var order = new Order
            {
                OrderCode = code,
                Status = "CREATED",
                PaymentMethod = Input.PaymentMethod,  // "COD" | "DIRECT"
                Subtotal = subtotal,
                ShippingFee = shipping,
                Discount = discount,
                Total = total,
                Note = Input.Note,
                FullName = Input.FullName,
                Phone = Input.Phone,
                Email = Input.Email,
                Address = Input.Address,
                Ward = Input.Ward,
                District = Input.District,
                Province = Input.Province
            };

            foreach (var it in items)
            {
                order.Items.Add(new OrderItem
                {
                    ProductId = it.Product.Id,
                    ProductName = it.Product.Name,
                    UnitPrice = it.Product.Price,
                    Quantity = it.Quantity
                });
            }

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            // clear cart
            HttpContext.Session.Remove(CartSessionKey);

            return RedirectToPage("/OrderSuccess", new { code });
        }

        // Helpers
        private async Task<(List<CartItem> items, decimal subtotal)> ReadCartAndRecalcAsync()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            var items = new List<CartItem>();
            decimal subtotal = 0;

            if (string.IsNullOrEmpty(cartJson)) return (items, subtotal);

            var cartDto = JsonSerializer.Deserialize<CartDto>(cartJson);
            if (cartDto == null || cartDto.Items.Count == 0) return (items, subtotal);

            foreach (var dto in cartDto.Items)
            {
                var p = await _db.Products.Include(p => p.Category)
                                          .FirstOrDefaultAsync(p => p.Id == dto.ProductId);
                if (p == null) continue;
                var ci = new CartItem { ProductId = p.Id, Product = p, Quantity = dto.Quantity };
                items.Add(ci);
                subtotal += p.Price * dto.Quantity; // dùng giá hiện tại
            }
            return (items, subtotal);
        }

        private static string GenerateOrderCode()
            => $"BT{DateTime.UtcNow:yyMMdd}{Guid.NewGuid().ToString("N")[..6].ToUpper()}";

        public class InputModel
        {
            [Required] public string FullName { get; set; } = "";
            [Required] public string Phone { get; set; } = "";
            public string? Email { get; set; }
            [Required] public string Address { get; set; } = "";
            public string? Ward { get; set; }
            public string? District { get; set; }
            public string? Province { get; set; }
            public string? Note { get; set; }
            [Required] public string PaymentMethod { get; set; } = "COD"; // COD/DIRECT
        }
        public class SummaryModel
        {
            public decimal Subtotal { get; set; }
            public decimal ShippingFee { get; set; }
            public decimal Discount { get; set; }
            public decimal Total => Subtotal + ShippingFee - Discount;
        }
    }
}
