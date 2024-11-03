using AutoParts_ShopAndForum.Infrastructure.Data.Constants;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoParts_ShopAndForum.Infrastructure.Data.Models
{
    public class User : IdentityUser
    {
        public User()
        {
            Orders = new HashSet<Order>();
        }

        [Required]
        [MaxLength(UserConstants.FirstNameMaxLength)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(UserConstants.LastNameMaxLength)]
        public string LastName { get; set; }

        [ForeignKey(nameof(Town))]
        public int TownId { get; set; }

        public virtual Town Town { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
