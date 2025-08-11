namespace AutoParts_ShopAndForum.Hub;

public interface IChatService
{
    void OnUserConnectedAsync(string connectionId, string userId, string userEmail, bool isSeller);
    void OnUserDisconnectedAsync(string connectionId);
    
    bool TryStartChatRequest(string initiatorId, string sellerId);
    bool TryAcceptChatRequest(string initiatorId, string sellerId, out string initiatorEmail);
    bool TryDeclineChatRequest(string initiatorId);
    
    bool TryStartPrivateChat(string initiatorId, string sellerId);
    bool TryEndPrivateChat(string sellerId);
    string TryEndPrivateChatBySeller(string sellerConnectionId);
    
    IEnumerable<ChatUser> GetAvailableSellers();
    IEnumerable<string> GetConnectionsByUserId(string userId);
    
    Task HandleMessageSend(
        string senderConnectionId, string receiverId, Func<ChatUser, IEnumerable<string>, Task> onMessageSend);

    IEnumerable<(string initiatorId, string sellerId)> GetExpiredChatRequests();
}