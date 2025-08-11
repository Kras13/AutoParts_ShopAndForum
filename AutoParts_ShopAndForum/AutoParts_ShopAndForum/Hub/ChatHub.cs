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

        chatService.OnUserConnectedAsync(Context.ConnectionId, userId, Context.User.GetEmail(), isSeller);

        await hubContext.Clients.All.SendAsync(
            HubConstants.UpdateSellersListHubMethod, chatService.GetAvailableSellers());

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        chatService.OnUserDisconnectedAsync(Context.ConnectionId);

        await hubContext.Clients.All.SendAsync(
            HubConstants.UpdateSellersListHubMethod, chatService.GetAvailableSellers());

        await base.OnDisconnectedAsync(exception);
    }

    public async Task RequestPrivateChat(string sellerId)
    {
        var initiatorId = Context.User.GetId();
        var initiatorEmail = Context.User.GetEmail();

        if (chatService.TryStartChatRequest(initiatorId, sellerId))
        {
            var sellerConnections = chatService.GetConnectionsByUserId(sellerId);

            await hubContext.Clients.Clients(sellerConnections)
                .SendAsync(HubConstants.ReceiveChatRequestHubMethod, initiatorId, initiatorEmail);
        }
        else
        {
            var initiatorConnections = chatService.GetConnectionsByUserId(initiatorId);

            await hubContext.Clients.Clients(initiatorConnections)
                .SendAsync(HubConstants.ReceiveSystemMessageHubMethod,
                    "Продавачът е зает в друг разговор или обработва заявка.");
        }
    }

    public async Task AcceptPrivateChat(string initiatorId)
    {
        var sellerId = Context.User.GetId();
        var sellerEmail = Context.User.GetEmail();

        if (chatService.TryAcceptChatRequest(initiatorId, sellerId, out var initiatorEmail))
        {
            var initiatorConnections = chatService.GetConnectionsByUserId(initiatorId);

            await hubContext.Clients.Clients(initiatorConnections)
                .SendAsync("ChatAccepted", sellerEmail);

            var sellerConnections = chatService.GetConnectionsByUserId(sellerId);

            await hubContext.Clients.Clients(sellerConnections)
                .SendAsync("ChatAccepted", initiatorEmail);
            
            await hubContext.Clients.All.SendAsync(HubConstants.UpdateSellersListHubMethod,
                chatService.GetAvailableSellers());
        }
    }

    public async Task DeclinePrivateChat(string initiatorId)
    {
        if (chatService.TryDeclineChatRequest(initiatorId))
        {
            var initiatorConnections = chatService.GetConnectionsByUserId(initiatorId);
            await hubContext.Clients.Clients(initiatorConnections)
                .SendAsync("ChatDeclined", "Продавачът отказа вашата заявка.");

            await hubContext.Clients.All.SendAsync(HubConstants.UpdateSellersListHubMethod,
                chatService.GetAvailableSellers());
        }
    }

    public async Task SendPrivateMessage(string receiverId, string message)
    {
        await chatService.HandleMessageSend(Context.ConnectionId, receiverId, async (sender, receiverConnections) =>
        {
            await hubContext.Clients.Clients(receiverConnections)
                .SendAsync(HubConstants.ReceivePrivateMessageHubMethod, sender.Id, sender.Email, message);
        });
    }

    public bool StartPrivateChat(string initiatorId, string sellerId)
    {
        var result = chatService.TryStartPrivateChat(initiatorId, sellerId);

        if (result)
        {
            hubContext.Clients.All.SendAsync(HubConstants.UpdateSellersListHubMethod,
                chatService.GetAvailableSellers());
        }

        return result;
    }

    public bool EndPrivateChat(string sellerId)
    {
        var result = chatService.TryEndPrivateChat(sellerId);

        if (result)
        {
            hubContext.Clients.All.SendAsync(HubConstants.UpdateSellersListHubMethod,
                chatService.GetAvailableSellers());
        }

        return result;
    }

    public async Task EndPrivateChatBySeller()
    {
        var customerId = chatService.TryEndPrivateChatBySeller(Context.ConnectionId);

        if (customerId != null)
        {
            var customerConnections = chatService.GetConnectionsByUserId(customerId);

            await hubContext.Clients.Clients(customerConnections)
                .SendAsync("ReceiveSystemMessage", "Продавачът напусна стаята.");

            await hubContext.Clients.All.SendAsync("UpdateSellersList", chatService.GetAvailableSellers());
        }
    }
}