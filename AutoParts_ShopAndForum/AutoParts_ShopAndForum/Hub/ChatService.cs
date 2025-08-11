namespace AutoParts_ShopAndForum.Hub;

public class ChatService : IChatService
{
    private static readonly Dictionary<string, ChatUser> UserConnections = new();
    private static readonly Dictionary<string, string> ReservedSellers = new();    
    private static readonly Dictionary<string, (string initiatorId, DateTime requestTime)> PendingChatRequests = new();

    private static readonly object StateLock = new();
    
    private static readonly TimeSpan ChatRequestTimeout = TimeSpan.FromSeconds(10);
    
    public void OnUserConnectedAsync(
        string connectionId, string userId, string userEmail, bool isSeller)
    {
        lock (StateLock)
        {
            UserConnections[connectionId] = new ChatUser(userId, userEmail, isSeller);
        }
    }

    public void OnUserDisconnectedAsync(string connectionId)
    {
        ChatUser chatUser;

        lock (StateLock)
        {
            UserConnections.Remove(connectionId, out chatUser);
        }

        if (chatUser == null)
            return;

        lock (StateLock)
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
    }

    public bool TryStartChatRequest(string initiatorId, string sellerId)
    {
        lock (StateLock)
        {
            if (ReservedSellers.ContainsKey(sellerId) || PendingChatRequests.ContainsKey(sellerId))
            {
                return false;
            }

            return PendingChatRequests.TryAdd(sellerId, (initiatorId, DateTime.UtcNow));
        }
    }

    public bool TryAcceptChatRequest(string initiatorId, string sellerId, out string initiatorEmail)
    {
        initiatorEmail = string.Empty;
        
        lock (StateLock)
        {
            if (!PendingChatRequests.TryGetValue(sellerId, out var requestData) || requestData.initiatorId != initiatorId)
            {
                return false;
            }
            
            initiatorEmail = UserConnections.FirstOrDefault(x => x.Value.Id == initiatorId).Value.Email;

            PendingChatRequests.Remove(sellerId);

            return ReservedSellers.TryAdd(sellerId, initiatorId);
        }
    }

    public bool TryDeclineChatRequest(string initiatorId, out string sellerId)
    {
        lock (StateLock)
        {
            sellerId = PendingChatRequests.FirstOrDefault(kvp => kvp.Value.initiatorId == initiatorId).Key;
            
            return sellerId != null && PendingChatRequests.Remove(sellerId);
        }
    }

    public bool TryStartPrivateChat(string initiatorId, string sellerId)
    {
        lock (StateLock)
        {
            return ReservedSellers.TryAdd(sellerId, initiatorId);
        }
    }

    public bool TryEndPrivateChat(string sellerId, out string initiatorId)
    {
        lock (StateLock)
        {
            return ReservedSellers.TryGetValue(sellerId, out initiatorId) && ReservedSellers.Remove(sellerId);
        }
    }

    public IEnumerable<ChatUser> GetAvailableSellers()
    {
        lock (StateLock)
        {
            return UserConnections
                .Where(user => user.Value.IsSeller && 
                               !ReservedSellers.ContainsKey(user.Value.Id) && 
                               !PendingChatRequests.ContainsKey(user.Value.Id))
                .Select(x => new ChatUser(x.Value.Id, x.Value.Email, x.Value.IsSeller))
                .DistinctBy(x => x.Id)
                .ToList();
        }
    }

    public Task HandleMessageSend(
        string senderConnectionId, string receiverId, Func<ChatUser, IEnumerable<string>, Task> onMessageSend)
    {
        IEnumerable<string> receiverConnections;
        ChatUser sender;

        lock (StateLock)
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

    public IEnumerable<(string initiatorId, string sellerId)> GetExpiredChatRequests()
    {
        lock (StateLock)
        {
            var expiredRequests = PendingChatRequests
                .Where(kvp => DateTime.UtcNow - kvp.Value.requestTime > ChatRequestTimeout)
                .ToList();

            foreach (var expired in expiredRequests)
            {
                PendingChatRequests.Remove(expired.Key);
            }
            
            return expiredRequests.Select(kvp => (kvp.Value.initiatorId, kvp.Key)).ToList();
        }
    }

    public string TryEndPrivateChatBySeller(string sellerConnectionId)
    {
        ChatUser seller;
        string customerId = null;

        lock (StateLock)
        {
            if (!UserConnections.TryGetValue(sellerConnectionId, out seller) || !seller.IsSeller)
            {
                return string.Empty;
            }
        
            ReservedSellers.Remove(seller.Id, out customerId);
        }

        return customerId;
    }

    public IEnumerable<string> GetConnectionsByUserId(string userId)
    {
        lock (StateLock)
        {
            return UserConnections
                .Where(x => x.Value.Id == userId)
                .Select(x => x.Key)
                .ToList();
        }
    }
}