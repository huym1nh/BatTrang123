namespace BatTrang.Models
{
    public class CartItem
    {
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public Product Product { get; set; } = null!;
        
        public decimal Subtotal => Product?.Price * Quantity ?? 0;
    }
}
