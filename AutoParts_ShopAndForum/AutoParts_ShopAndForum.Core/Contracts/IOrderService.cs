using AutoParts_ShopAndForum.Core.Models.Cart;
using AutoParts_ShopAndForum.Core.Models.Order;

namespace AutoParts_ShopAndForum.Core.Contracts
{
    public interface IOrderService
    {
        OrderSummaryModel[] GetAllByUserId(string userId);
        OrderModel PlaceOrderAndClearCart(ref ICollection<ProductCartModel> cart, OrderInputModel inputModel);
        OrderModel FindByPublicToken(Guid orderToken);
        int MarkOnlinePaymentAsSuccessful(Guid orderToken);
        int MarkOnlinePaymentAsCancelled(Guid orderToken);
        OrderDetailsModel GetOrderDetails(int orderId, string userId);
    }
}
