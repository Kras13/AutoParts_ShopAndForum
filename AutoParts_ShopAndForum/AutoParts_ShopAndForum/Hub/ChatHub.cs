namespace AutoParts_ShopAndForum.Hub;

using System;
using Infrastructure;
using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
    private readonly IChatService _chatService;
    private readonly IHubContext<ChatHub> _hubContext;
    private Timer _chatRequestTimer;

    public ChatHub(IChatService chatService, IHubContext<ChatHub> hubContext)
    {
        _chatService = chatService;
        _hubContext = hubContext;

        _chatRequestTimer = new Timer(
            async (_) => { await HandleExpiredRequests(); }, 
            null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User.GetId();
        var isSeller = Context.User.IsSeller() || Context.User.IsAdmin();

        _chatService.OnUserConnectedAsync(Context.ConnectionId, userId, Context.User.GetEmail(), isSeller);

        await _hubContext.Clients.All.SendAsync(
            HubConstants.UpdateSellersListHubMethod, _chatService.GetAvailableSellers());

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        _chatService.OnUserDisconnectedAsync(Context.ConnectionId);

        await _hubContext.Clients.All.SendAsync(
            HubConstants.UpdateSellersListHubMethod, _chatService.GetAvailableSellers());

        await base.OnDisconnectedAsync(exception);
    }

    public async Task RequestPrivateChat(string sellerId)
    {
        var initiatorId = Context.User.GetId();
        var initiatorEmail = Context.User.GetEmail();

        if (_chatService.TryStartChatRequest(initiatorId, sellerId))
        {
            var sellerConnections = _chatService.GetConnectionsByUserId(sellerId);

            await _hubContext.Clients.Clients(sellerConnections)
                .SendAsync(HubConstants.ReceiveChatRequestHubMethod, initiatorId, initiatorEmail);
        }
        else
        {
            var initiatorConnections = _chatService.GetConnectionsByUserId(initiatorId);

            await _hubContext.Clients.Clients(initiatorConnections)
                .SendAsync(HubConstants.ReceiveSystemMessageHubMethod,
                    "Продавачът е зает в друг разговор или обработва заявка.");
        }
    }

    public async Task AcceptPrivateChat(string initiatorId)
    {
        var sellerId = Context.User.GetId();

        if (_chatService.TryAcceptChatRequest(initiatorId, sellerId, out _)) // todo -> might be without out parameter
        {
            var initiatorConnections = _chatService.GetConnectionsByUserId(initiatorId);

            await _hubContext.Clients.Clients(initiatorConnections)
                .SendAsync("ChatAccepted", sellerId);

            var sellerConnections = _chatService.GetConnectionsByUserId(sellerId);

            await _hubContext.Clients.Clients(sellerConnections)
                .SendAsync("ChatAccepted", initiatorId);

            await _hubContext.Clients.All.SendAsync(HubConstants.UpdateSellersListHubMethod,
                _chatService.GetAvailableSellers());
        }
    }

    public async Task DeclinePrivateChat(string initiatorId)
    {
        if (_chatService.TryDeclineChatRequest(initiatorId, out var sellerId))
        {
            var initiatorConnections = _chatService.GetConnectionsByUserId(initiatorId);
            var sellerConnections = _chatService.GetConnectionsByUserId(sellerId);
            
            await _hubContext.Clients.Clients(initiatorConnections)
                .SendAsync("ChatDeclined", "Продавачът отказа вашата заявка.");
            
            await _hubContext.Clients.Clients(sellerConnections)
                .SendAsync("ChatDeclined", "Успешно отказана заявка.");

            await _hubContext.Clients.All.SendAsync(HubConstants.UpdateSellersListHubMethod,
                _chatService.GetAvailableSellers());
        }
    }

    public async Task SendPrivateMessage(string receiverId, string message)
    {
        await _chatService.HandleMessageSend(Context.ConnectionId, receiverId, async (sender, receiverConnections) =>
        {
            await _hubContext.Clients.Clients(receiverConnections)
                .SendAsync(HubConstants.ReceivePrivateMessageHubMethod, sender.Id, sender.Email, message);
        });
    }

    public bool StartPrivateChat(string initiatorId, string sellerId)
    {
        var result = _chatService.TryStartPrivateChat(initiatorId, sellerId);

        if (result)
        {
            _hubContext.Clients.All.SendAsync(HubConstants.UpdateSellersListHubMethod,
                _chatService.GetAvailableSellers());
        }

        return result;
    }

    public async Task<bool> EndPrivateChat(string sellerId)
    {
        var result = _chatService.TryEndPrivateChat(sellerId, out var initiatorId);

        if (result)
        {
            var sellerConnections = _chatService.GetConnectionsByUserId(sellerId);
            
            await _hubContext.Clients.All.SendAsync(HubConstants.UpdateSellersListHubMethod,
                _chatService.GetAvailableSellers());
            
            await _hubContext.Clients.Clients(sellerConnections)
                .SendAsync("ReceiveSystemMessage", "Клиентът напусна стаята.");
        }

        return result;
    }

    public async Task EndPrivateChatBySeller()
    {
        var customerId = _chatService.TryEndPrivateChatBySeller(Context.ConnectionId);

        if (customerId != null)
        {
            var customerConnections = _chatService.GetConnectionsByUserId(customerId);

            await _hubContext.Clients.Clients(customerConnections)
                .SendAsync("ReceiveSystemMessage", "Продавачът напусна стаята.");

            await _hubContext.Clients.All.SendAsync("UpdateSellersList", _chatService.GetAvailableSellers());
        }
    }

    private async Task HandleExpiredRequests()
    {
        var expiredRequests = _chatService.GetExpiredChatRequests();
        var hasExpiredRequest = false;

        foreach (var expired in expiredRequests)
        {
            var initiatorConnections = _chatService.GetConnectionsByUserId(expired.initiatorId);
            
            await _hubContext.Clients.Clients(initiatorConnections)
                .SendAsync("ChatDeclined", "Времето за отговор на заявката ви изтече.");

            hasExpiredRequest = true;
        }

        if (hasExpiredRequest)
        {
            await _hubContext.Clients.All.SendAsync(HubConstants.UpdateSellersListHubMethod,
                _chatService.GetAvailableSellers());
        }
    }
}