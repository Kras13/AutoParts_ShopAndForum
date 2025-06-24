using AutoParts_ShopAndForum.Core.Models.Cart;

namespace AutoParts_ShopAndForum.Models.Checkout;

public class CheckoutItemsModel
{
    public ICollection<ProductCartModel> Products { get; set; }
    public decimal Total { get; set; }
    public decimal TransportValue { get; set; }
}