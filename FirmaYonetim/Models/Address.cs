using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FirmaYonetim.Models
{
    public class Address
    {
        public Guid? Id { get; set; }
        public Guid CompanyId { get; set; }
        public Guid CreatedByUserId { get; set; }
        public string Name { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string AddressDetail { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Phone3 { get; set; }
        public string Email { get; set; }
        public bool IsDelete { get; set; }
        public DateTime? IsDeleteDateTime { get; set; }
        public DateTime CreatedDateTime { get; set; }

        public User CreatedByUser { get; set; }
        public Company Company { get; set; }

        public List<Contact> ContactList { get; set; }

    }
}