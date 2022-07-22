using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace WinService.Models
{
    public class Activity
    {
        public Guid? Id { get; set; }
        public Guid UserId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid AddressId { get; set; }
        public Guid ContactId { get; set; }
        public Guid ActivityTypeId { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime? Date { get; set; }
        public int Status { get; set; }
        public DateTime? StatusUpdateDateTime { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string editDateTime { get; set; }
        public bool IsMailSend { get; set; }

        public User User { get; set; }
        public Company Company { get; set; }
        public Address Address { get; set; }
        public Contact Contact { get; set; }
        public ActivityType ActivityType { get; set; }
    }
}