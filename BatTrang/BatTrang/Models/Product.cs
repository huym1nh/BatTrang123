using System;
using System.Collections.Generic;

namespace BatTrang.Models;

public partial class Product
{
    public long Id { get; set; }

    public long CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public virtual Category Category { get; set; } = null!;
}
