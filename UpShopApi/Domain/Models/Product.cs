namespace UpShopApi.Domain.Models
{
    public class Product
    {
        public string Id { get; set; } = default!;
        public string Sku { get; set; } = default!;
        public string Name { get; set; } = default!;
        public double Price { get; set; }
        public int AvailableQuantity { get; set; }
        public string? ImageUrl { get; set; }
        public bool Featured { get; set; }
        public bool OnSpecial { get; set; }
    }
}
