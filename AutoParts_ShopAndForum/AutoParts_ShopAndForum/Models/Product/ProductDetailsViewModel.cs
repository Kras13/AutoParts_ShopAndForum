using System.ComponentModel.DataAnnotations;

namespace AutoParts_ShopAndForum.Models.Product
{
    public class ProductDetailsViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public decimal Price { get; set; }

        public string Description { get; set; }

        public string LastUrl { get; set; } = string.Empty;

        [Range(1, 10)]
        public int Quantity { get; set; } = 1;

        public bool AddedToCart { get; set; }
    }
}
