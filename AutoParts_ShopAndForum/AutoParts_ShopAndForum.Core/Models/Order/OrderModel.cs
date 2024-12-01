using AutoParts_ShopAndForum.Core.Models.Cart;

namespace AutoParts_ShopAndForum.Core.Models.Order
{
    public class OrderModel
    {
        public int Id { get; set; }

        public string Street { get; set; }

        public string Town { get; set; }

        public ProductCartModel[] Products { get; set; }
    }
}
