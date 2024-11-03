namespace AutoParts_ShopAndForum.Infrastructure.Data.Constants
{
    internal class UserConstants
    {
        internal const int FirstNameMaxLength = 32;
        internal const int LastNameMaxLength = 32;
    }

    internal class TownConstants
    {
        internal const int NameMaxLength = 64;
    }

    internal class OrderConstants
    {
        internal const int StreetMaxLength = 256;
    }

    internal class ProductCategoryConstants
    {
        internal const int NameMaxLength = 128;
    }

    internal class ProductSubcategoryConstants
    {
        internal const int NameMaxLength = 128;
    }

    internal class ProductConstants
    {
        internal const int NameMaxLength = 128;
        internal const int DescriptionMaxLength = 1024 * 1024;
    }
}
