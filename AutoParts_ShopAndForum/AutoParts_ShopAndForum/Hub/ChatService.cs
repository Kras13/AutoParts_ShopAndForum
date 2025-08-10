using Microsoft.AspNetCore.SignalR;

namespace AutoParts_ShopAndForum.Hub;

public class ChatService: IChatService
{
    private static readonly Dictionary<string, ChatUser> UserConnections = new();
    private static readonly Dictionary<string, string> ReservedSellers = new();

    private static readonly object ReservedSellersLock = new();
    private static readonly object UserConnectionsLock = new();

    public Task OnUserConnectedAsync(
        string connectionId, string userId, string userEmail, bool isSeller)
    {
        lock (UserConnectionsLock)
        {
            UserConnections[connectionId] = new ChatUser(userId, userEmail, isSeller);
        }

        return Task.CompletedTask;
    }

    public Task OnUserDisconnectedAsync(string connectionId)
    {
        ChatUser chatUser;

        lock (UserConnectionsLock)
        {
            UserConnections.Remove(connectionId, out chatUser);
        }

        if (chatUser == null)
            return Task.CompletedTask;

        lock (ReservedSellersLock)
        {
            if (chatUser.IsSeller)
            {
                ReservedSellers.Remove(chatUser.Id);
            }
            else
            {
                var sellerId = ReservedSellers.FirstOrDefault(kvp => kvp.Value == chatUser.Id).Key;

                if (sellerId != null)
                    ReservedSellers.Remove(sellerId);
            }
        }

        return Task.CompletedTask;
    }

    public bool TryStartPrivateChat(string initiatorId, string sellerId)
    {
        lock (ReservedSellersLock)
        {
            return ReservedSellers.TryAdd(sellerId, initiatorId);
        }
    }

    public bool TryEndPrivateChat(string sellerId)
    {
        lock (ReservedSellersLock)
        {
            return ReservedSellers.Remove(sellerId);
        }
    }

    public IEnumerable<ChatUser> GetAvailableSellers()
    {
        lock (UserConnectionsLock)
        {
            lock (ReservedSellersLock)
            {
                return UserConnections
                    .Where(user => user.Value.IsSeller && !ReservedSellers.ContainsKey(user.Value.Id))
                    .Select(x => new ChatUser(x.Value.Id, x.Value.Email, x.Value.IsSeller))
                    .DistinctBy(x => x.Id)
                    .ToList();
            }
        }
    }

    public Task HandleMessageSend(
        string senderConnectionId, string receiverId, Func<ChatUser, IEnumerable<string>, Task> onMessageSend)
    {
        IEnumerable<string> receiverConnections;
        ChatUser sender;

        lock (UserConnectionsLock)
        {
            receiverConnections = UserConnections
                .Where(x => x.Value.Id == receiverId)
                .Select(x => x.Key)
                .ToList();

            if (!UserConnections.TryGetValue(senderConnectionId, out sender))
                return Task.CompletedTask;
        }
        
        return onMessageSend(sender, receiverConnections);
    }
}