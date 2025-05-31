using System.ComponentModel.DataAnnotations;
using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string ConcurrencyStamp { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string FirstName { get; set; } // New property
        public string LastName { get; set; } // New property
    }
}
