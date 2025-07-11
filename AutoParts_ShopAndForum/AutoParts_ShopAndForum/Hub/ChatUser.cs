namespace AutoParts_ShopAndForum.Hub;

public class ChatUser(string id, string email, bool isSeller)
{
    public string Id { get; } = id;
    public bool IsSeller { get; } = isSeller;
    public string Email { get; } = email;
}