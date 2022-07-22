using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FirmaYonetim.Models
{
    public class SharedCompany
    {

        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public Guid WhoSharedUserId { get; set; }
        public Guid SeeUserId { get; set; }

        public User seeUser { get; set; }
        public Company company { get; set; }
    }
}