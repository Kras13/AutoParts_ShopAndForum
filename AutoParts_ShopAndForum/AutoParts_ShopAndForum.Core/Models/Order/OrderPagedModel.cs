using AutoParts_ShopAndForum.Core.Models.Cart;

namespace AutoParts_ShopAndForum.Core.Models.Order
{
    public class OrderPagedModel
    {
        public ICollection<OrderModel> Orders { get; set; }
        
        public int TotalProductsWithoutPagination { get; set; }
    }
}
