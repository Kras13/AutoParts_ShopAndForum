using System.Collections.Concurrent;

namespace AutoParts_ShopAndForum.Hub;

using System;
using Infrastructure;
using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
    private static readonly ConcurrentDictionary<string, ChatUser> UserConnections = new();
    private static readonly ConcurrentDictionary<string, string> ReservedSellers = new();
    
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
        
        if (chatUser.IsSeller)
        {
            ReservedSellers.TryRemove(chatUser.Id, out _);
        }
        else
        {
            var sellerId = ReservedSellers.FirstOrDefault(kvp => kvp.Value == chatUser.Id).Key;
            
            ReservedSellers.TryRemove(sellerId, out _);
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
        if (ReservedSellers.ContainsKey(sellerId))
            return false;
        
        ReservedSellers.TryAdd(sellerId, initiatorId);

        BroadcastSellersList();

        return true; // todo - think if the sellers to be kept in one connection only (bad if multiple tabs opened)
    }
    
    public bool EndPrivateChat(string sellerId)
    {
        var result = ReservedSellers.TryRemove(sellerId, out _);

        BroadcastSellersList();
        
        return result;
    }
    
    public async Task SendPrivateMessage(string senderId, string receiverId, string message)
    {
        var receiverConnections = UserConnections
            .Where(x => x.Value.Id == receiverId)
            .Select(x => x.Key);
        
        var senderEmail = UserConnections.FirstOrDefault(x => x.Value.Id == senderId).Value.Email;
        
        await Clients.Clients(receiverConnections).SendAsync("ReceivePrivateMessage", senderId, senderEmail, message);
    }
}