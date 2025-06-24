using AutoParts_ShopAndForum.Core.Models.Town;

namespace AutoParts_ShopAndForum.Core.Models.CourierStation;

public class CourierStationModel
{
    public int Id { get; set; }
    public string FullAddress { get; set; }
    public TownModel Town { get; set; }
    public CourierStationType Type { get; set; }
    public string DisplayName => $"{FullAddress} - [{(Type == CourierStationType.Office ? "ОФИС" : "АВТОМАТ")}]";
}