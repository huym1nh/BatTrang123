namespace BatTrang.Models
{
    public class AddToCartRequest
    {
        public long ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
