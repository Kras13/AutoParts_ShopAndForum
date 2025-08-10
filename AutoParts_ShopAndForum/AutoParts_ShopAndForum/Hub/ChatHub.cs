using System.Collections.Concurrent;

namespace AutoParts_ShopAndForum.Hub;

using System;
using Infrastructure;
using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
    private static readonly ConcurrentDictionary<string, ChatUser> UserConnections = new();
    private static readonly ConcurrentDictionary<string, string> ReservedSellers = new();
    private static readonly object ReservedSellersLock = new();
    private static readonly object UserConnectionsLock = new();

    public override Task OnConnectedAsync()
    {
        var userId = Context.User.GetId();
        var isSeller = Context.User.IsSeller() || Context.User.IsAdmin();

        UserConnections[Context.ConnectionId] = new ChatUser(
            userId, Context.User.GetEmail(), isSeller);

        BroadcastSellersList();

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        UserConnections.Remove(Context.ConnectionId, out var chatUser);

        if (chatUser == null)
            return base.OnDisconnectedAsync(exception);

        lock (ReservedSellersLock)
        {
            if (chatUser.IsSeller)
            {
                ReservedSellers.TryRemove(chatUser.Id, out _);
            }
            else
            {
                var sellerId = ReservedSellers.FirstOrDefault(kvp => kvp.Value == chatUser.Id).Key;

                if (sellerId != null)
                {
                    ReservedSellers.TryRemove(sellerId, out _);
                }
            }
        }

        BroadcastSellersList();

        return base.OnDisconnectedAsync(exception);
    }

    private Task BroadcastSellersList()
    {
        var availableSellers = UserConnections
            .Where(user => user.Value.IsSeller && !ReservedSellers.ContainsKey(user.Value.Id))
            .Select(x => new { id = x.Value.Id, name = x.Value.Email })
            .DistinctBy(x => x.id) // one user might have multiple connections...must appear once
            .ToList();

        return Clients.All.SendAsync("UpdateSellersList", availableSellers);
    }

    public bool StartPrivateChat(string initiatorId, string sellerId)
    {
        var result = ReservedSellers.TryAdd(sellerId, initiatorId);

        if (result)
            BroadcastSellersList();

        return result;
    }

    public bool EndPrivateChat(string sellerId)
    {
        var result = ReservedSellers.TryRemove(sellerId, out _);

        BroadcastSellersList();

        return result;
    }

    public async Task SendPrivateMessage(string receiverId, string message)
    {
        IEnumerable<string> receiverConnections;
        ChatUser sender;

        lock (UserConnectionsLock)
        {
            receiverConnections = UserConnections
                .Where(x => x.Value.Id == receiverId)
                .Select(x => x.Key)
                .ToList();

            if (!UserConnections.TryGetValue(Context.ConnectionId, out sender))
                return;
        }

        await Clients.Clients(receiverConnections).SendAsync("ReceivePrivateMessage", sender.Id, sender.Email, message);
    }
}