using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Entities;
public class Color : BaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string HexCode { get; set; }
    public bool IsActive { get; set; }
}
