using AutoParts_ShopAndForum.Core.Models.Cart;

namespace AutoParts_ShopAndForum.Models.Checkout;

public class CheckoutModel
{
    public string BoundFromPersonalReceive { get; set; }
    public bool PersonalReceive { get; set; }
    public DeliveryMethod DeliveryMethod { get; set; }
    public ICollection<ProductCartModel> Products { get; set; }
}