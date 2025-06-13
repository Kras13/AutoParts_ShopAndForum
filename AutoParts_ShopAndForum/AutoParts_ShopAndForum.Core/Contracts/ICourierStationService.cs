using AutoParts_ShopAndForum.Core.Models.CourierStation;

namespace AutoParts_ShopAndForum.Core.Contracts;

public interface ICourierStationService
{
    ICollection<CourierStationModel> GetAllByTownId(int townId);
}