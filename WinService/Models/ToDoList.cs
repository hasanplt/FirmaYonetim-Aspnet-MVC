using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WinService.Models
{
    public class ToDoList
    {
        public Guid? Id { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public bool IsDelete { get; set; }
        public DateTime? IsDeleteDateTime { get; set; }

        public string EditDateTime { get; set; }
        public int dateRangeHours { get; set; }
    }
}