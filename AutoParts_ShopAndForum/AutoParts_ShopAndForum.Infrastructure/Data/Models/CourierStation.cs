using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AutoParts_ShopAndForum.Infrastructure.Data.Constants;

namespace AutoParts_ShopAndForum.Infrastructure.Data.Models;

public class CourierStation
{
    public int Id { get; set; }
    
    [MaxLength(CourierStationConstants.TitleMaxLength)]
    public string Title { get; set; }
    
    [MaxLength(CourierStationConstants.FullAddressMaxLength)]
    public string FullAddress { get; set; }
    
    [ForeignKey(nameof(User))]
    public int TownId { get; set; }
    
    public Town Town { get; set; }
    
    public CourierStationType Type { get; set; }

    public CourierStationFirm Firm { get; set; }
}