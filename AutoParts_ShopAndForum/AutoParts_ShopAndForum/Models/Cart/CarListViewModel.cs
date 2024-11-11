using AutoParts_ShopAndForum.Core.Models.Cart;
using AutoParts_ShopAndForum.Core.Models.Town;
using System.ComponentModel;

namespace AutoParts_ShopAndForum.Models.Cart
{
    public class CartListViewModel
    {
        public ICollection<ProductCartModel> Products { get; set; }

        [DisplayName("Town")]
        public ICollection<TownModel> Towns { get; set; }

        public int SelectedTownId { get; set; }
    }
}
