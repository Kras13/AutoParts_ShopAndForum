namespace AutoParts_ShopAndForum.Hub;

using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
    public async Task SendMessage(string senderId, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", senderId, message);
    }
}