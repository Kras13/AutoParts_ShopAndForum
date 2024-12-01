using AutoParts_ShopAndForum.Core.Models.Order;

namespace AutoParts_ShopAndForum.Core.Contracts
{
    public interface IOrderService
    {
        OrderModel[] GetAllByUserId(string userId);
    }
}
