using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Task01_Module01.Models
{
    public class UserInfo
    {
        public int UserId { get; set; }
        public string Country { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string CompanyName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long? Phone { get; set; }
        public string PhotoFileName { get; set; }
        public string ShopAddress { get; set; }
        public long? BankAccount { get; set; }
    }
}
