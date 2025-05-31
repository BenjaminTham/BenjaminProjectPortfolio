using Ecommerce.Domain.Common;
using System.Xml.Linq;

namespace Ecommerce.Domain.Entities;
public class AppConfiguration : BaseEntity
{
    public int Id { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
}