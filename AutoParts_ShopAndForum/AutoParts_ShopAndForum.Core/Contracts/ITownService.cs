using AutoParts_ShopAndForum.Core.Models.Town;

namespace AutoParts_ShopAndForum.Core.Contracts
{
    public interface ITownService
    {
        TownModel[] GetAll();
    }
}
