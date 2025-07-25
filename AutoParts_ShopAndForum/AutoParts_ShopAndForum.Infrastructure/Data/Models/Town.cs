﻿using AutoParts_ShopAndForum.Infrastructure.Data.Constants;
using System.ComponentModel.DataAnnotations;

namespace AutoParts_ShopAndForum.Infrastructure.Data.Models
{
    public class Town
    {
        public Town()
        {
            Users = new HashSet<User>();
            Orders = new HashSet<Order>();
            CourierStations = new HashSet<CourierStation>();
        }

        public int Id { get; set; }

        [Required]
        [MaxLength(TownConstants.NameMaxLength)]
        public string Name { get; set; }
        
        [MaxLength(TownConstants.PostCodeMaxLength)]
        public string PostCode { get; set; }

        public bool? IsCity { get; set; }

        public virtual ICollection<Order> Orders { get; set; }

        public virtual ICollection<User> Users { get; set; }
        
        public virtual ICollection<CourierStation> CourierStations { get; set; }
    }
}
