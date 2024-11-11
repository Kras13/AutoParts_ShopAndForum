using AutoParts_ShopAndForum.Core.Models.Cart;

namespace AutoParts_ShopAndForum.Core.Contracts
{
    public interface ICartService
    {
        void Add(ref ICollection<ProductCartModel> cart, ProductCartModel product);

        void ChangeQuantity(ref ICollection<ProductCartModel> cart, int productId, int quantity);

        int Order(ref ICollection<ProductCartModel> cart, string userId, string street, int townId);
    }
}
