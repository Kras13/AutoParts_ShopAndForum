namespace AutoParts_ShopAndForum.Hub;

public interface IChatService
{
    Task OnUserConnectedAsync(string connectionId, string userId, string userEmail, bool isSeller);
    Task OnUserDisconnectedAsync(string connectionId);
    
    bool TryStartPrivateChat(string initiatorId, string sellerId);
    bool TryEndPrivateChat(string sellerId);
    
    IEnumerable<ChatUser> GetAvailableSellers();
    Task HandleMessageSend(
        string senderConnectionId, string receiverId, Func<ChatUser, IEnumerable<string>, Task> onMessageSend);
}