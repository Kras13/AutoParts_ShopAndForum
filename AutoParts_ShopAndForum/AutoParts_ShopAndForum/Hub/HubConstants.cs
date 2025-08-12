namespace AutoParts_ShopAndForum.Hub;

public class HubConstants
{
    public const string UpdateSellersListHubMethod = "UpdateSellersList";
    public const string ReceivePrivateMessageHubMethod = "ReceivePrivateMessage";
    public const string ReceiveChatRequestHubMethod = "ReceiveChatRequest";
    public const string ReceiveSystemMessageHubMethod = "ReceiveSystemMessage";
    public const string ChatAcceptedHubMethod = "ChatAccepted";
    public const string ChatDeclinedHubMethod = "ChatDeclined";
}