namespace AutoParts_ShopAndForum.Core.Contracts;

public interface IOrderNotificationService
{
    void SendNotification(string receiverEmail, string firstName);
}