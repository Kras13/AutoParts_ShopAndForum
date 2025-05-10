using System.Collections.Concurrent;

namespace AutoParts_ShopAndForum.Hub;

using System;
using AutoParts_ShopAndForum.Infrastructure;
using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
    private static readonly ConcurrentDictionary<string, ChatUser> ConnectedUsers = new();
    private static readonly ConcurrentDictionary<string, ChatUser> AvailableSellers = new();
    
    public override Task OnConnectedAsync()
    {
        var userId = Context.User.GetId();
        var isSeller = Context.User.IsSeller() || Context.User.IsAdmin();
            
        ConnectedUsers[Context.ConnectionId] = new ChatUser(userId, Context.User.GetName(), isSeller);
    
        if (isSeller)
            AvailableSellers[Context.ConnectionId] = new ChatUser(userId, Context.User.GetName(), true);
        
        BroadcastSellersList();
        
        return base.OnConnectedAsync();
    }
    
    public override Task OnDisconnectedAsync(Exception exception)
    {
        ConnectedUsers.Remove(Context.ConnectionId, out _);
        AvailableSellers.Remove(Context.ConnectionId, out _);
    
        BroadcastSellersList();
    
        return base.OnDisconnectedAsync(exception);
    }
    
    private Task BroadcastSellersList()
    {
        var availableSellers = AvailableSellers
            .Select(x => x.Value.Id)
            .Distinct() // one user might have multiple connections...must appear once
            .ToList();
    
        return Clients.All.SendAsync("UpdateSellersList", availableSellers);
    }

    public bool StartPrivateChat(string userId)
    {
        var sellerConnections = AvailableSellers
            .Where(x => x.Value.Id == userId);

        foreach (var sellerConnection in sellerConnections)
        {
            AvailableSellers.Remove(sellerConnection.Key, out _); // no longer available for other users
        }

        BroadcastSellersList();

        return true; // todo - think if the sellers to be kept in one connection only (bad if multiple tabs opened)
    }
    
    public async Task SendPrivateMessage(string senderId, string receiverId, string message)
    {
        var receiverConnections = ConnectedUsers
            .Where(x => x.Value.Id == receiverId)
            .Select(x => x.Key);
        
        await Clients.Clients(receiverConnections).SendAsync("ReceivePrivateMessage", senderId, message);
    }
}