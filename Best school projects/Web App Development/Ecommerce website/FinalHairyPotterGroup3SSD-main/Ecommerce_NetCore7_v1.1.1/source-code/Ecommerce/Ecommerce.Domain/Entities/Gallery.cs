using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Entities;
public class Gallery : BaseEntity
{
    public string Id { get; set; }
    public string? Title { get; set; }
    public string? Name { get; set; }
    public string? Tags { get; set; }
}

