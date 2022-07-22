using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WinService.Models
{
    public class Company
    {
        public Guid? Id { get; set; }
        public Guid UpdatedByUserId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public Guid CreatedByUserId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime? UpdateDateTime { get; set; }
        public bool IsDelete { get; set; }
        public DateTime? IsDeleteDateTime { get; set; }

        public User CreatedByUser { get; set; }
        public User UpdatedByUser { get; set; } 

        public List<Address> AddressList { get; set; }
        public List<Contact> ContactList { get; set; }
        public List<Activity> ActivityList { get; set; }
    }
}