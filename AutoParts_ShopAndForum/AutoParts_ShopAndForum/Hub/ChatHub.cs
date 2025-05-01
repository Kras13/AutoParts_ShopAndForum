using System.Collections.Concurrent;

namespace AutoParts_ShopAndForum.Hub;

using System;
using AutoParts_ShopAndForum.Infrastructure;
using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
    private static readonly ConcurrentDictionary<string, (string UserId, bool IsAvailable, bool IsSeller)> ConnectedUsers = new();
    

    public override Task OnConnectedAsync()
    {
        var userId = Context.User.GetId();
        
        if (!Context.User.IsAdmin() && !Context.User.IsSeller())
        {
            ConnectedUsers[Context.ConnectionId] = (userId, true, false);
            
            BroadcastUserList();

            return base.OnConnectedAsync();
        }

        ConnectedUsers[Context.ConnectionId] = (userId, true, true);

        BroadcastUserList();

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        if (!Context.User.IsAdmin() && !Context.User.IsSeller())
            return base.OnDisconnectedAsync(exception);

        ConnectedUsers.Remove(Context.ConnectionId, out _);

        BroadcastUserList();

        return base.OnDisconnectedAsync(exception);
    }

    private Task BroadcastUserList()
    {
        var availableUsers = ConnectedUsers
            .Where(x => x.Value is { IsAvailable: true, IsSeller: true })
            .Select(x => x.Value.UserId)
            .Distinct() // one user might have multiple connections...must appear once
            .ToList();

        return Clients.All.SendAsync("UpdateUserList", availableUsers);
    }

    public async Task SendPrivateMessage(string senderId, string receiverId, string message)
    {
        var receiverConnectionId = ConnectedUsers.FirstOrDefault(x => x.Value.UserId == receiverId).Key;
        
        await Clients.Client(receiverConnectionId).SendAsync("ReceivePrivateMessage", senderId, message);
    }
}