namespace AutoParts_ShopAndForum.Hub;

using System;
using AutoParts_ShopAndForum.Infrastructure;
using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
    private static readonly Dictionary<string, (string UserId, bool IsAvailable)> ConnectedSellers = new();

    public override Task OnConnectedAsync()
    {
        if (!Context.User.IsAdmin() && !Context.User.IsSeller())
        {
            BroadcastUserList();

            return base.OnConnectedAsync();
        }

        var userId = Context.User.GetId();

        lock (ConnectedSellers)
        {
            ConnectedSellers[Context.ConnectionId] = (userId, true);
        }

        BroadcastUserList();

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        if (!Context.User.IsAdmin() && !Context.User.IsSeller())
            return base.OnDisconnectedAsync(exception);

        lock (ConnectedSellers)
        {
            ConnectedSellers.Remove(Context.ConnectionId);
        }

        BroadcastUserList();

        return base.OnDisconnectedAsync(exception);
    }

    private Task BroadcastUserList()
    {
        var availableUsers = ConnectedSellers
            .Where(x => x.Value.IsAvailable)
            .Select(x => x.Value.UserId)
            .Distinct() // one user might have multiple connections...must appear once
            .ToList();

        return Clients.All.SendAsync("UpdateUserList", availableUsers);
    }

    public async Task SendMessage(string senderId, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", senderId, message);
    }
}