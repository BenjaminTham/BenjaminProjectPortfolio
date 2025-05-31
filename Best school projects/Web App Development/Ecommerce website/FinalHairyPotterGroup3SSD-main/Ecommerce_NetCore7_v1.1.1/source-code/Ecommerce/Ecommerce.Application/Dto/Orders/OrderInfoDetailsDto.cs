using Ecommerce.Application.Dto;
using Ecommerce.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Application.Dto;

public class OrderInfoDetailsDto
{
    public List<OrderItemDetailsDto> OrderItemDetails { get; set; }
    public UpdateOrderStatusDto UpdateOrderStatus { get; set; }
    public Order? Order { get; set; }
    public List<OrderStatus>? OrderStatus { get; set; }
    public OrderStatus? CurrentOrderStatus { get; set; }
    public CustomerDto? CustomerInfo { get; set; }
}
public class OrderItemDetailsDto
{
    public long ProductId { get; set; }
    public string ProductSlug { get; set; }
    public long ProductVariantId { get; set; }
    public string? OrderItemTitle { get; set; }
    public string? OrderItemImage { get; set; }
    public decimal ItemUnitPrice { get; set; }
    public int ItemQty { get; set; }
}
public class UpdateOrderStatusDto
{
    public long OrderId { get; set; }
    public string? InvoiceNo { get; set; }
    public string? CurrentOrderStatus { get; set; }
    public int NewOrderStatus { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9\s.,!?_'-]*$", ErrorMessage = "Only alphanumeric characters, spaces, '.', ',', '!', '?', '-', and '_' are allowed in the description.")]
    public string? Description { get; set; }
    public DateTime? CurrentOrderStatusTime { get; set; }
}

public class UpdateOrderShippingInfoDto
{
    public long OrderId { get; set; }

    [RegularExpression("^[A-Za-z]+(?: [A-Za-z]+)*$", ErrorMessage = "Name must contain only letters.")]
    public string CustomerName { get; set; }

    [Phone(ErrorMessage = "Invalid phone number")]
    public string CustomerMobile { get; set; }


    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string CustomerEmail { get; set; }


    [RegularExpression(@"^[a-zA-Z0-9\s,.'#-]{3,}$", ErrorMessage = "Shipping address must only contain alphanumeric characters, spaces, #, and common punctuation.")]
    public string ShippingAddress { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9\s.,!?_'-]*$", ErrorMessage = "Only alphanumeric characters, spaces, '.', ',', '!', '?', '-', and '_' are allowed in the description.")]
    public string? CustomerComment { get; set; }
}
