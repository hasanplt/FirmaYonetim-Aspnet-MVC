using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FirmaYonetim.Models
{
    public class ViewModel
    {
        public int? countCompany { get; set; }
        public int? countContact { get; set; }
        public int? countActivityWaiting { get; set; }
        public int? countActivityFinish { get; set; }
        public string searchKeyword { get; set; }
        public User user { get; set; }
        public Company company { get; set; }
        public Contact contact { get; set; }
        public Activity activity { get; set; }
        public ActivityType activityType { get; set; }
        public Address address { get; set; }
        public ToDoList toDo { get; set; }
        public List<ToDoList> toDoList { get; set; }
        public List<ActivityType> activityTypeList { get; set; }
        public List<Activity> activityList { get; set; }
        public List<Company> companyList { get; set; }
        public List<Contact> contactList { get; set; }
        public List<Address> addressList { get; set; }
    }
}