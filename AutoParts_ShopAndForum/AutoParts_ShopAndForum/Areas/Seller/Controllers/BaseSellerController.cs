using AutoParts_ShopAndForum.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoParts_ShopAndForum.Areas.Seller.Controllers
{
    [Area(RoleType.Seller)]
    [Authorize(Roles = RoleType.Seller + "," + RoleType.Administrator)]
    public abstract class BaseSellerController : Controller
    {
    }
}
