using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FirmaYonetim.Models
{
    public class ActivityType
    {
        public Guid? Id { get; set; }
        public string Text { get; set; }
        public bool IsDelete { get; set; }
        public DateTime? IsDeleteDateTime { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public DateTime CreatedDateTime { get; set; }

        public List<Activity> ActivityList { get; set; }
    }
}