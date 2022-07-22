using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WinService.Models
{
    public class UserWrongLoginLog
    {
        public Guid? Id { get; set; }
        public Guid UserId { get; set; }
        public string WrongPassword { get; set; }
        public DateTime CreateDateTime { get; set; }
    }
}