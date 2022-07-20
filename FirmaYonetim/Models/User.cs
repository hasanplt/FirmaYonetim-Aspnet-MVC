using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FirmaYonetim.Models
{
    public class User
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int LockCount { get; set; }
        public DateTime? LockDateTime { get; set; }
        public DateTime CreateDateTime { get; set; }

        public List<UserWrongLoginLog> UserWrongLoginLogs { get; set; }
    }
}