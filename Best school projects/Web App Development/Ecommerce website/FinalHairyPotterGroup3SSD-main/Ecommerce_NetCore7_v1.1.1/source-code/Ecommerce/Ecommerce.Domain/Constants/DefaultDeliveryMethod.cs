using Ecommerce.Domain.Entities;

namespace Ecommerce.Domain.Constants;

public static class DefaultDeliveryMethod
{
    public static DeliveryMethod HomeDelivery() { return new DeliveryMethod { Name = "Home Delivery", Price = 0, Description = "Product will shipped to customer destination", IsActive = true }; }
    public static DeliveryMethod StorePickup() { return new DeliveryMethod { Name = "Shop Pickup", Price = 0, Description = "Customer will collect product from Store", IsActive = true }; }

    public static List<DeliveryMethod> GetDefaultDeliveryMethod()
    {
        var defaultDeliveryMethod = new List<DeliveryMethod>();
        defaultDeliveryMethod.Add(HomeDelivery());
        defaultDeliveryMethod.Add(StorePickup());
        return defaultDeliveryMethod;
    }
}

