using AutoParts_ShopAndForum.Core.Contracts;
using AutoParts_ShopAndForum.Core.Models.Town;
using AutoParts_ShopAndForum.Infrastructure.Data;

namespace AutoParts_ShopAndForum.Core.Services
{
    public class TownService : ITownService
    {
        private readonly ApplicationDbContext _context;

        public TownService(ApplicationDbContext context)
        {
            _context = context;
        }

        public TownModel[] GetAll()
        {
            return _context.Towns
                .Select(m => new TownModel() { Id = m.Id, Name = m.Name })
                .ToArray();
        }
    }
}
