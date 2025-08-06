using AutoParts_ShopAndForum.Core.Contracts;
using AutoParts_ShopAndForum.Core.Models.Order;
using AutoParts_ShopAndForum.Localization;
using AutoParts_ShopAndForum.Models;

namespace AutoParts_ShopAndForum.Areas.Administrator.Models.Order;

public class OrderQueryViewModel
{
    public int CurrentPage { get; set; }

    public PageSizeViewModel[] Pages { get; set; } =
    [
        new() { SizeDisplayText = "9", Size = 9 },
        new() { SizeDisplayText = "12", Size = 12 },
        new() { SizeDisplayText = "15", Size = 15 },
        new() { SizeDisplayText = "18", Size = 18 },
        new() { SizeDisplayText = MainLocalization.OrderQueryViewModel_AllPages, Size = IOrderService.AllOrders }
    ];

    public int OrdersPerPage { get; set; } = 9;
    public int TotalOrders { get; set; }
    public OrdersSorting Sorting { get; set; }

    public OrderSortingViewModel[] Sortings { get; set; } =
    [
        new() { DisplayText = MainLocalization.OrderQueryViewModel_NoSorting, Sorting = OrdersSorting.NoSorting },
        new()
        {
            DisplayText = MainLocalization.OrderQueryViewModel_TotalAscending, Sorting = OrdersSorting.OverallSumAscending
        },
        new()
        {
            DisplayText = MainLocalization.OrderQueryViewModel_TotalDescending, Sorting = OrdersSorting.OverallSumDescending
        },
        new()
        {
            DisplayText = MainLocalization.OrderQueryViewModel_DateCreatedAscending,
            Sorting = OrdersSorting.DateCreatedAscending
        },
        new()
        {
            DisplayText = MainLocalization.OrderQueryViewModel_DateCreatedDescending,
            Sorting = OrdersSorting.DateCreatedDescending
        },
        new()
        {
            DisplayText = MainLocalization.OrderQueryViewModel_DateDeliveredAscending,
            Sorting = OrdersSorting.DateDeliveredAscending
        },
        new()
        {
            DisplayText = MainLocalization.OrderQueryViewModel_DateDeliveredDescending,
            Sorting = OrdersSorting.DateDeliveredDescending
        },
    ];

    public OrderStatusFilter StatusFilter { get; set; } = OrderStatusFilter.All;

    public OrderSummaryModel[] Orders { get; set; }
}