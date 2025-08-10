using System.Collections.Concurrent;

namespace AutoParts_ShopAndForum.Hub;

using System;
using Infrastructure;
using Microsoft.AspNetCore.SignalR;

public class ChatHub(IChatService chatService, IHubContext<ChatHub> hubContext) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User.GetId();
        var isSeller = Context.User.IsSeller() || Context.User.IsAdmin();

        await chatService.OnUserConnectedAsync(Context.ConnectionId, userId, Context.User.GetEmail(), isSeller);
        await hubContext.Clients.All.SendAsync("UpdateSellersList", chatService.GetAvailableSellers());

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await chatService.OnUserDisconnectedAsync(Context.ConnectionId);
        await hubContext.Clients.All.SendAsync("UpdateSellersList", chatService.GetAvailableSellers());

        await base.OnDisconnectedAsync(exception);
    }

    public bool StartPrivateChat(string initiatorId, string sellerId)
    {
        var result = chatService.TryStartPrivateChat(initiatorId, sellerId);

        if (result)
            hubContext.Clients.All.SendAsync("UpdateSellersList", chatService.GetAvailableSellers());

        return result;
    }

    public bool EndPrivateChat(string sellerId)
    {
        var result = chatService.TryEndPrivateChat(sellerId);

        if (result)
        {
            hubContext.Clients.All.SendAsync("UpdateSellersList", chatService.GetAvailableSellers());
        }

        return result;
    }

    public async Task SendPrivateMessage(string receiverId, string message)
    {
        await chatService.HandleMessageSend(Context.ConnectionId, receiverId, async (sender, receiverConnections) =>
        {
            await hubContext.Clients.Clients(receiverConnections)
                .SendAsync("ReceivePrivateMessage", sender.Id, sender.Email, message);
        });
    }
}