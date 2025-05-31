using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Entities;
public class Size : BaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
}
