using Play.Inventory.Service.Dtos;

namespace Play.Inventory.Service;

public static class Extensions
{
    public static InventoryItemDto AsDto(this InventoryItemDto item)
    {
        return new InventoryItemDto(item.CatalogItemId, item.Quantity, item.AcquiredDate);
    }
}