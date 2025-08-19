using AutoParts_ShopAndForum.Core.Models.Cart;
using AutoParts_ShopAndForum.Core.Models.Order;
using Stripe.Checkout;

namespace AutoParts_ShopAndForum.Core.Contracts
{
    public interface IOrderService
    {
        const int AllOrders = -1;
        OrderPagedModel GetAllByUserId(string userId, int pageNumber, int pageSize);
        OrderModel PlaceOrderAndClearCart(ref ICollection<ProductCartModel> cart, OrderInputModel inputModel);
        OrderModel FindByPublicToken(Guid orderToken);
        int MarkOnlinePaymentAsSuccessful(Guid orderToken);
        int MarkOnlinePaymentAsCancelled(Guid orderToken);
        OrderDetailsModel GetOrderDetails(int orderId, string userId);
        OrderQueryModel GetQueried(
            int currentPage, int ordersPerPage, OrdersSorting sorting, OrderStatusFilter statusFilter);
        OrderModel UpdateOrder(OrderEditModel orderEditModel);
        SessionCreateOptions CreateStripeSession(string successUrl, string cancelUrl, Guid orderPublicToken);
    }
}
