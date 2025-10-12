namespace BatTrang.Models
{
    public class CartDto
    {
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
        
        public decimal Total => Items.Sum(x => x.Subtotal);
        
        public int TotalItems => Items.Sum(x => x.Quantity);
    }

    public class CartItemDto
    {
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public ProductDto Product { get; set; } = null!;
        
        public decimal Subtotal => Product?.Price * Quantity ?? 0;
    }

    public class ProductDto
    {
        public long Id { get; set; }
        public long CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public CategoryDto? Category { get; set; }
    }

    public class CategoryDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
