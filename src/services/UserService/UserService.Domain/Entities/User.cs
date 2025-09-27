using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } 
        public string PhoneNumber { get; set; } 
        public string AvatarUrl { get; set; } 
        public string Gender { get; set; } = string.Empty;
    }
}
