using AutoParts_ShopAndForum.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoParts_ShopAndForum.Areas.Administrator.Controllers;

[Area(RoleType.Administrator)]
[Authorize(Roles = RoleType.Administrator)]
public abstract class BaseAdminController : Controller
{
}