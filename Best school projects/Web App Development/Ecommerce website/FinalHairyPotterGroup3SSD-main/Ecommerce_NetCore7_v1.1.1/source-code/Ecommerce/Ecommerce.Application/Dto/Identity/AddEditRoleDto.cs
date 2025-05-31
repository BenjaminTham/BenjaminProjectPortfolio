using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Application.Dto;

public class AddEditRoleDto
{
    public string Id { get; set; }
    [Required]
    [RegularExpression(@"^[a-zA-Z0-9\s.,!?_]*$", ErrorMessage = "Only alphanumeric characters, spaces, '.', ',', '!', '?', and '_' are allowed.")]
    public string Name { get; set; }
}
