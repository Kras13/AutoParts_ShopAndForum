using AutoParts_ShopAndForum.Core.Models.EmailNotification;

namespace AutoParts_ShopAndForum.Core.Contracts;

public interface IOrderNotificationService
{
    Task SendNotificationAsync(
        string receiverEmail, string firstName, OrderEmailModel model);
}