using AutoParts_ShopAndForum.Core.Contracts;
using AutoParts_ShopAndForum.Core.Models.CourierStation;
using AutoParts_ShopAndForum.Core.Models.Town;
using AutoParts_ShopAndForum.Infrastructure.Data;

namespace AutoParts_ShopAndForum.Core.Services;

public class CourierStationService(ApplicationDbContext context) : ICourierStationService
{
    public ICollection<CourierStationModel> GetAllByTownId(int townId)
    {
        var town = context.Towns
            .FirstOrDefault(t => t.Id == townId);

        if (town == null)
            throw new ArgumentException("Town with such id doesn't exist");

        return context.CourierStations
            .Where(c => c.TownId == townId)
            .Select(x => new CourierStationModel
            {
                Id = x.Id,
                FullAddress = x.FullAddress,
                Town = new TownModel
                {
                    Id = x.TownId,
                    Name = x.Town.Name,
                },
                Type = MapToModelType(x.Type)
            })
            .ToArray();
    }

    private static Core.Models.CourierStation.CourierStationType MapToModelType(
        Infrastructure.Data.Models.CourierStationType type)
    {
        switch (type)
        {
            case Infrastructure.Data.Models.CourierStationType.Office:
                return CourierStationType.Office;
            case Infrastructure.Data.Models.CourierStationType.Machine:
                return CourierStationType.Machine;
        }

        throw new ArgumentException("Invalid station type.");
    }
}