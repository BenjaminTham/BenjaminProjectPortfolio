using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Application.Dto;

using System.ComponentModel.DataAnnotations;

public class CheckoutDto
{
    public int DeliveryMethod { get; set; }
    public string PaymentMethod { get; set; }

    [Required(ErrorMessage = "First Name Required!")]
    [Display(Name = "First Name")]
    [RegularExpression(@"^[a-zA-Z0-9\s.,!?]*$", ErrorMessage = "Only alphanumeric characters, spaces, '.', ',', '!', and '?' are allowed in first name.")]
    public string CustomerFirstName { get; set; }

    [Required(ErrorMessage = "Last Name Required!")]
    [Display(Name = "Last Name")]
    [RegularExpression(@"^[a-zA-Z0-9\s.,!?]*$", ErrorMessage = "Only alphanumeric characters, spaces, '.', ',', '!', and '?' are allowed in last name.")]
    public string CustomerLastName { get; set; }

    public string CustomerFullName => $"{CustomerFirstName} {CustomerLastName}";

    [Required(ErrorMessage = "Phone No. Required!")]
    [Display(Name = "Phone Number")]
    public string CustomerPhoneNumber { get; set; }

    [Required(ErrorMessage = "Email Required!")]
    [Display(Name = "Email")]
    [EmailAddress(ErrorMessage = "Invalid Email Address!")]
    public string CustomerEmail { get; set; }

    [Required(ErrorMessage = "Shipping Address Required!")]
    [Display(Name = "Shipping Address")]
    [RegularExpression(@"^[a-zA-Z0-9\s.,!?#]*$", ErrorMessage = "Only alphanumeric characters, spaces, '.', ',', '!', '?', and '#' are allowed in the shipping address.")]
    public string ShippingAddress { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9\s.,!?]*$", ErrorMessage = "Only alphanumeric characters, spaces, '.', ',', '!', and '?' are allowed in comments.")]
    public string? CustomerComment { get; set; }

    public StripeModel? StripeModel { get; set; }
    public PaypalModel? PaypalModel { get; set; }
    public bool WillSaveInfo { get; set; }
}

public class StripeModel
{
    public string? Token { get; set; }
    [RegularExpression(@"^[a-zA-Z0-9\s.,!?]*$", ErrorMessage = "Only alphanumeric characters, spaces, '.', ',', '!', and '?' are allowed in comments.")]
    public string? NameOnCard { get; set; }
}

public class PaypalModel
{
    public string? TransactionId { get; set; }

    public string? AccountName { get; set; }
    [Required(ErrorMessage = "Email Required!")]
    [Display(Name = "Email")]
    [EmailAddress(ErrorMessage = "Invalid Email Address!")]
    public string? AccountEmail { get; set; }
}
