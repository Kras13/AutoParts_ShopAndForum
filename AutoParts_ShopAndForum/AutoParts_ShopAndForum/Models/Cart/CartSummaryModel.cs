using AutoParts_ShopAndForum.Core.Models.Cart;

namespace AutoParts_ShopAndForum.Models.Cart;

public class CartSummaryModel
{
    public ICollection<ProductCartModel> Products { get; set; }
    public decimal Total { get; set; }
}