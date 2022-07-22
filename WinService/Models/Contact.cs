using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WinService.Models
{
    public class Contact
    {
        public Guid? Id { get; set; }
        public Guid AddressId { get; set; }
        public Guid CreatedByUserId { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string GSM { get; set; }
        public string LandPhone { get; set; }
        public string LandPhoneInternal { get; set; }
        public bool IsDelete { get; set; }
        public DateTime? IsDeleteDateTime { get; set; }
        public DateTime CreatedDateTime { get; set; }

        public User User { get; set; }
        public Address Address { get; set; }

        public List<Activity> ActivityList { get; set; }
    }
}