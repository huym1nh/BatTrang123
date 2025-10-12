using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BatTrang.Models;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace BatTrang.Pages
{
    public class CartModel : PageModel
    {
        private readonly ExeContext _db;
        private const string CartSessionKey = "Cart";

        public CartModel(ExeContext db)
        {
            _db = db;
        }

        public Cart Cart { get; set; } = new Cart();

        public async Task OnGetAsync()
        {
            Cart = await GetCartAsync();
        }

        private async Task<Cart> GetCartAsync()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartJson))
            {
                return new Cart();
            }

            var cartDto = JsonSerializer.Deserialize<CartDto>(cartJson);
            if (cartDto == null)
            {
                return new Cart();
            }

            // Convert DTO back to Cart
            var cart = new Cart();
            foreach (var itemDto in cartDto.Items)
            {
                var product = await _db.Products
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.Id == itemDto.ProductId) ?? new Product();
                
                cart.AddItem(product, itemDto.Quantity);
            }

            return cart;
        }

        private Cart GetCart()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartJson))
            {
                return new Cart();
            }

            var cartDto = JsonSerializer.Deserialize<CartDto>(cartJson);
            if (cartDto == null)
            {
                return new Cart();
            }

            // Convert DTO back to Cart
            var cart = new Cart();
            foreach (var itemDto in cartDto.Items)
            {
                var product = _db.Products
                    .Include(p => p.Category)
                    .FirstOrDefault(p => p.Id == itemDto.ProductId) ?? new Product();
                
                cart.AddItem(product, itemDto.Quantity);
            }

            return cart;
        }

        private void SaveCart(Cart cart)
        {
            // Convert to DTO to avoid circular reference
            var cartDto = new CartDto
            {
                Items = cart.Items.Select(item => new CartItemDto
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Product = new ProductDto
                    {
                        Id = item.Product.Id,
                        CategoryId = item.Product.CategoryId,
                        Name = item.Product.Name,
                        ImageUrl = item.Product.ImageUrl,
                        Description = item.Product.Description,
                        Price = item.Product.Price,
                        Category = item.Product.Category != null ? new CategoryDto
                        {
                            Id = item.Product.Category.Id,
                            Name = item.Product.Category.Name
                        } : null
                    }
                }).ToList()
            };
            
            var cartJson = JsonSerializer.Serialize(cartDto);
            HttpContext.Session.SetString(CartSessionKey, cartJson);
        }

        public IActionResult OnPostAddToCart([FromBody] AddToCartRequest request)
        {
            try
            {
                // Debug: Log request
                Console.WriteLine($"Attempting to add product ID: {request.ProductId}, Quantity: {request.Quantity}");
                
                // Get all products for debugging
                var allProducts = _db.Products.ToList();
                Console.WriteLine($"Total products in database: {allProducts.Count}");
                
                var product = _db.Products
                    .Include(p => p.Category)
                    .FirstOrDefault(p => p.Id == request.ProductId);

                if (product == null)
                {
                    Console.WriteLine($"Product with ID {request.ProductId} not found");
                    return new JsonResult(new { 
                        success = false, 
                        message = $"Sản phẩm với ID {request.ProductId} không tồn tại. Tổng sản phẩm: {allProducts.Count}" 
                    });
                }

                Console.WriteLine($"Found product: {product.Name}");
                
                var cart = GetCart();
                cart.AddItem(product, request.Quantity);
                SaveCart(cart);

                return new JsonResult(new { 
                    success = true, 
                    message = "Đã thêm vào giỏ hàng", 
                    cartCount = cart.TotalItems 
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in OnPostAddToCart: {ex.Message}");
                return new JsonResult(new { 
                    success = false, 
                    message = $"Lỗi: {ex.Message}" 
                });
            }
        }

        public IActionResult OnPostRemoveFromCart([FromBody] RemoveFromCartRequest request)
        {
            var cart = GetCart();
            cart.RemoveItem(request.ProductId);
            SaveCart(cart);

            return new JsonResult(new { 
                success = true, 
                message = "Đã xóa khỏi giỏ hàng",
                cartCount = cart.TotalItems
            });
        }

        public IActionResult OnPostUpdateQuantity([FromBody] UpdateQuantityRequest request)
        {
            var cart = GetCart();
            cart.UpdateQuantity(request.ProductId, request.Quantity);
            SaveCart(cart);

            var item = cart.Items.FirstOrDefault(x => x.ProductId == request.ProductId);
            return new JsonResult(new { 
                success = true, 
                message = "Đã cập nhật số lượng",
                cartCount = cart.TotalItems,
                subtotal = item?.Subtotal ?? 0,
                total = cart.Total
            });
        }

        public IActionResult OnGetCartCount()
        {
            var cart = GetCart();
            return new JsonResult(new { cartCount = cart.TotalItems });
        }

        public IActionResult OnPostClearCart()
        {
            var cart = GetCart();
            cart.Clear();
            SaveCart(cart);

            return new JsonResult(new { 
                success = true, 
                message = "Đã xóa tất cả sản phẩm khỏi giỏ hàng",
                cartCount = 0
            });
        }
    }
}
