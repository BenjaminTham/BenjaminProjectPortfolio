using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Entities;
public class ContactQuery : BaseEntity
{
    public long Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Subject { get; set; }
    public string MessageBody { get; set; }
}
