using AutoParts_ShopAndForum.Core.Contracts;
using AutoParts_ShopAndForum.Core.Models.EmailNotification;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace AutoParts_ShopAndForum.Service;

public class OrderNotificationService(
    IEmailSender emailSender, RazorViewToStringRenderer razorRenderer) : IOrderNotificationService
{
    public async Task SendNotificationAsync(
        string receiverEmail, string firstName, OrderEmailModel model)
    {
        var htmlContent = await razorRenderer.RenderViewToStringAsync(
            "/Views/Emails/OrderConfirmation.cshtml", model);

        await emailSender.SendEmailAsync(
            receiverEmail, $"Потвърждение на поръчка - #{model.OrderId}", htmlContent);
    }
}
