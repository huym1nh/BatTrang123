using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BatTrang.Models;

public partial class ExeContext : DbContext
{
    public ExeContext()
    {
    }

    public ExeContext(DbContextOptions<ExeContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning Connection string should be provided via DI; this fallback is only used if not configured.
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("server=DESKTOP-F4JA3S4;database=BatTrangHamony;uid=sa;pwd=123;TrustServerCertificate=True;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__categori__3213E83F1D65AF10");

            entity.ToTable("categories");

            entity.HasIndex(e => e.Name, "UQ__categori__72E12F1BD351AB1E").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__products__3213E83F1AF34E09");

            entity.ToTable("products");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Description)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("image_url");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("price");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__products__catego__2A4B4B5E");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__users__3213E83F2BAA3C07");

            entity.ToTable("users");

            entity.HasIndex(e => e.Username, "UQ__users__F3DBC57202484484").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("username");
        });

        modelBuilder.Entity<Order>(e =>
        {
            e.ToTable("orders");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");

            e.Property(x => x.OrderCode).HasColumnName("order_code").HasMaxLength(32).IsRequired();
            e.Property(x => x.UserId).HasColumnName("user_id");
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20).IsRequired();
            e.Property(x => x.PaymentMethod).HasColumnName("payment_method").HasMaxLength(10).IsRequired();

            e.Property(x => x.Subtotal).HasColumnName("subtotal").HasColumnType("decimal(12,2)");
            e.Property(x => x.ShippingFee).HasColumnName("shipping_fee").HasColumnType("decimal(12,2)");
            e.Property(x => x.Discount).HasColumnName("discount").HasColumnType("decimal(12,2)");
            e.Property(x => x.Total).HasColumnName("total").HasColumnType("decimal(12,2)");
            e.Property(x => x.Note).HasColumnName("note").HasMaxLength(500);

            e.Property(x => x.FullName).HasColumnName("full_name").HasMaxLength(150).IsRequired();
            e.Property(x => x.Phone).HasColumnName("phone").HasMaxLength(30).IsRequired();
            e.Property(x => x.Email).HasColumnName("email").HasMaxLength(200);
            e.Property(x => x.Address).HasColumnName("address").HasMaxLength(500).IsRequired();
            e.Property(x => x.Ward).HasColumnName("ward").HasMaxLength(200);
            e.Property(x => x.District).HasColumnName("district").HasMaxLength(200);
            e.Property(x => x.Province).HasColumnName("province").HasMaxLength(200);

            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.PaidAt).HasColumnName("paid_at");
        });

        // === OrderItem ===
        modelBuilder.Entity<OrderItem>(e =>
        {
            e.ToTable("order_items");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");

            e.Property(x => x.OrderId).HasColumnName("order_id");
            e.Property(x => x.ProductId).HasColumnName("product_id");
            e.Property(x => x.ProductName).HasColumnName("product_name").HasMaxLength(200).IsRequired();
            e.Property(x => x.UnitPrice).HasColumnName("unit_price").HasColumnType("decimal(12,2)");
            e.Property(x => x.Quantity).HasColumnName("quantity");

            e.HasOne(x => x.Order).WithMany(o => o.Items).HasForeignKey(x => x.OrderId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
