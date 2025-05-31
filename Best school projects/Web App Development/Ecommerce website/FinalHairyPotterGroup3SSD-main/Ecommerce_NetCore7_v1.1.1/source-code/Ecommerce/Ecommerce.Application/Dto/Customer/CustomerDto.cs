using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Application.Dto;

public class CustomerDto
{
    public long Id { get; set; }

    public string? UserFullName { get; set; }
    public string? UserProfilePicture { get; set; }
    public string? UserGender { get; set; }
    public DateTime? CreatedDate { get; set; }



    [RegularExpression("^[A-Za-z]+(?: [A-Za-z]+)*$", ErrorMessage = "First name must contain only letters.")]
    public string UserFirstName { get; set; }

    [RegularExpression("^[A-Za-z]+(?: [A-Za-z]+)*$", ErrorMessage = "Last name must contain only letters.")]
    public string UserLastName { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string? UserEmail { get; set; }

    [Phone(ErrorMessage = "Invalid phone number")]
    public string? UserPhoneNumber { get; set; }




    [RegularExpression(@"^[a-zA-Z0-9\s,.'#-]{3,}$", ErrorMessage = "Shipping address must only contain alphanumeric characters, spaces, #, and common punctuation.")]
    public string? ShippingAddress { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9\s,.'#-]{3,}$", ErrorMessage = "Billing address must only contain alphanumeric characters, spaces, #, and common punctuation.")]
    public string? BillingAddress { get; set; }


}
