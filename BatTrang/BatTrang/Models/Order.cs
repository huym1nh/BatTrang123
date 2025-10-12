using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BatTrang.Models
{
    public class Order
    {
        public long Id { get; set; }

        [Required, MaxLength(32)]
        public string OrderCode { get; set; } = "";

        public long? UserId { get; set; }   // nếu có đăng nhập thì map

        [Required, MaxLength(20)]
        public string Status { get; set; } = "CREATED"; // CREATED/PAID/CANCELLED

        [Required, MaxLength(10)]
        public string PaymentMethod { get; set; } = "COD"; // COD/DIRECT

        [Column(TypeName = "decimal(12,2)")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal ShippingFee { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal Discount { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal Total { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }

        // Shipping info
        [Required, MaxLength(150)] public string FullName { get; set; } = "";
        [Required, MaxLength(30)] public string Phone { get; set; } = "";
        [MaxLength(200)] public string? Email { get; set; }
        [Required, MaxLength(500)] public string Address { get; set; } = "";
        [MaxLength(200)] public string? Ward { get; set; }
        [MaxLength(200)] public string? District { get; set; }
        [MaxLength(200)] public string? Province { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PaidAt { get; set; }

        public List<OrderItem> Items { get; set; } = new();
    }

    public class OrderItem
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public Order Order { get; set; } = default!;

        public long ProductId { get; set; }
        [Required, MaxLength(200)]
        public string ProductName { get; set; } = "";

        [Column(TypeName = "decimal(12,2)")]
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}
