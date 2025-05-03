namespace AutoParts_ShopAndForum.Hub;

public class ChatUser(string id, string name, bool isSeller)
{
    public string Id { get; set; } = id;
    public string Name { get; set; } = name;
    public bool IsSeller { get; set; } = isSeller;
}