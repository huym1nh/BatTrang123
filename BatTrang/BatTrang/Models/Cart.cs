namespace BatTrang.Models
{
    public class Cart
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        
        public decimal Total => Items.Sum(x => x.Subtotal);
        
        public int TotalItems => Items.Sum(x => x.Quantity);
        
        public void AddItem(Product product, int quantity = 1)
        {
            var existingItem = Items.FirstOrDefault(x => x.ProductId == product.Id);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                Items.Add(new CartItem
                {
                    ProductId = product.Id,
                    Quantity = quantity,
                    Product = product
                });
            }
        }
        
        public void RemoveItem(long productId)
        {
            Items.RemoveAll(x => x.ProductId == productId);
        }
        
        public void UpdateQuantity(long productId, int quantity)
        {
            var item = Items.FirstOrDefault(x => x.ProductId == productId);
            if (item != null)
            {
                if (quantity <= 0)
                {
                    RemoveItem(productId);
                }
                else
                {
                    item.Quantity = quantity;
                }
            }
        }
        
        public void Clear()
        {
            Items.Clear();
        }
    }
}
