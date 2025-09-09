using AutoParts_ShopAndForum.Core.Contracts;
using AutoParts_ShopAndForum.Core.Models.EmailNotification;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace AutoParts_ShopAndForum.Service;

public class OrderNotificationService(
    IEmailSender emailSender, RazorViewToStringRenderer razorRenderer) : IOrderNotificationService
{
    private readonly IEmailSender _emailSender = emailSender;
    private readonly RazorViewToStringRenderer _razorRenderer = razorRenderer;

    public async Task SendNotificationAsync(
        string receiverEmail, string firstName, OrderEmailModel model)
    {
        var htmlContent = await _razorRenderer.RenderViewToStringAsync("/Views/Emails/OrderConfirmation.cshtml", model);

        await _emailSender.SendEmailAsync(
            receiverEmail, $"Order Confirmation - #{model.OrderId}", htmlContent);
    }
}
