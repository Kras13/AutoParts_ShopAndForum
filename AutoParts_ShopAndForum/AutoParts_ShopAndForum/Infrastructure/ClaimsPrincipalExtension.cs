using AutoParts_ShopAndForum.Infrastructure.Data;
using System.Security.Claims;

namespace AutoParts_ShopAndForum.Infrastructure
{
    public static class ClaimsPrincipalExtension
    {
        public static string GetId(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            return user.IsInRole(RoleType.Administrator);
        }

        public static bool IsSeller(this ClaimsPrincipal user)
        {
            return user.IsInRole(RoleType.Seller);
        }

        public static string GetName(this ClaimsPrincipal user)
        {
            return user.FindFirst("FirstName")?.Value;
        }

        public static string GetEmail(this ClaimsPrincipal user)
        {
            return user.FindFirst("Emails")?.Value;
        }
    }
}
