using Ecommerce.Domain.Entities;

namespace Ecommerce.Domain.Constants;

public static class DefaultOrderStatusValue
{
    public static OrderStatusValue Pending() { return new OrderStatusValue { StatusValue = "Pending", Description = "Waiting For Order Confirmation" }; }
    public static OrderStatusValue Accepted() { return new OrderStatusValue { StatusValue = "Accepted", Description = "Order Accepted" }; }
    public static OrderStatusValue PendingPayment() { return new OrderStatusValue { StatusValue = "Pending Payment", Description = "Order Received, Waiting for Payment" }; }
    public static OrderStatusValue PaymentReceived() { return new OrderStatusValue { StatusValue = "Payment Received", Description = "Payment Received For Order" }; }
    public static OrderStatusValue Processing() { return new OrderStatusValue { StatusValue = "Processing", Description = "Order Processing For Shipment" }; }
    public static OrderStatusValue Shipped() { return new OrderStatusValue { StatusValue = "Shipped", Description = "Order Is Shipped For Delivery" }; }
    public static OrderStatusValue Delivered() { return new OrderStatusValue { StatusValue = "Delivered", Description = "Order Delivered" }; }
    //public static OrderStatusValue Completed() { return new OrderStatusValue { StatusValue = "Completed", Description = "Order Completed" }; }
    public static OrderStatusValue Cancelled() { return new OrderStatusValue { StatusValue = "Cancelled", Description = "Order Cancelled" }; }


    public static List<OrderStatusValue> GetDefaultStatus()
    {
        var defaultOrderStatus = new List<OrderStatusValue>();
        defaultOrderStatus.Add(Pending());
        defaultOrderStatus.Add(PendingPayment());
        defaultOrderStatus.Add(Accepted());
        defaultOrderStatus.Add(Processing());
        defaultOrderStatus.Add(Shipped());
        defaultOrderStatus.Add(Delivered());
        //defaultOrderStatus.Add(Completed());
        defaultOrderStatus.Add(Cancelled());
        defaultOrderStatus.Add(PaymentReceived());
        return defaultOrderStatus;
    }

}

