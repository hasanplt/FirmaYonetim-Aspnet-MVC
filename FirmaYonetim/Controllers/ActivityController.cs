using Dapper;
using FirmaYonetim.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FirmaYonetim.Controllers
{
    public class ActivityController : Controller
    {
        IDbConnection conn = Connection.conn;

        // GET: Activity
        public ActionResult Index()
        {
            if (Session["user"] == null) return RedirectToAction("Index", "Login");

            conn.Open();

            List<Activity> activities = conn.Query<Activity>("SELECT * FROM Activity").ToList();
            List<ActivityType> activityTypes = conn.Query<ActivityType>("SELECT * FROM ActivityType WHERE IsDelete = @IsDelete" , new ActivityType() { IsDelete = false }).ToList();

            conn.Close();

            activities = editActivitiesAddDetails(activities);

            ViewModel model = new ViewModel();
            model.activityTypeList = activityTypes;
            model.activityList = activities;
            model.user = PublicFunctions.getUser(conn, Session["user"].ToString());
            return View(model);
        }
        public ActionResult Add()
        {
            if (Session["user"] == null) return RedirectToAction("Index", "Login");
            User user = PublicFunctions.getUser(conn, Session["user"].ToString());

            conn.Open();
            
            List<ActivityType> activityTypes = conn.Query<ActivityType>("SELECT * FROM ActivityType WHERE IsDelete = @IsDelete", new ActivityType() { IsDelete = false }).ToList();
            List<Company> companies = conn.Query<Company>("SELECT * FROM Company WHERE IsDelete = @IsDelete and CreatedByUserId = @CreatedByUserId", new Company() { IsDelete = false, CreatedByUserId = (Guid)user.Id }).ToList();
            
            conn.Close();

            ViewModel model = new ViewModel();
            model.activityTypeList = activityTypes;
            model.companyList = companies;
            model.user = user;
            return View(model);
        }
        public ActionResult Detail(Guid? Id)
        {
            if (Id == null) return RedirectToAction("Index", "Home");
            if (Session["user"] == null) return RedirectToAction("Index", "Login");

            User user = PublicFunctions.getUser(conn, Session["user"].ToString());

            conn.Open();
            
            List<ActivityType> activityTypes = conn.Query<ActivityType>("SELECT * FROM ActivityType WHERE IsDelete = @IsDelete", new ActivityType() { IsDelete = false }).ToList();
            List<Company> companies = conn.Query<Company>("SELECT * FROM Company WHERE IsDelete = @IsDelete and CreatedByUserId = @CreatedByUserId", new Company() { IsDelete = false, CreatedByUserId = (Guid)user.Id }).ToList();
            Activity activity = conn.Query<Activity>("SELECT * FROM Activity WHERE Id = @Id", new Activity() { Id = Id }).FirstOrDefault();
            List<Address> addresses = conn.Query<Address>("SELECT * FROM Address WHERE IsDelete = @IsDelete and CompanyId = @CompanyId", new Address() { IsDelete = false, CompanyId = activity.CompanyId }).ToList();
            List<Contact> contacts = conn.Query<Contact>("SELECT * FROM Contact WHERE IsDelete = @IsDelete and AddressId = @AddressId", new Contact() { IsDelete = false, AddressId = activity.AddressId }).ToList();
            
            conn.Close();

            activity.editDateTime = PublicFunctions.dateTimeToStringEdit((DateTime)activity.Date);

            ViewModel model = new ViewModel();
            model.activityTypeList = activityTypes;
            model.activity = activity;
            model.companyList = companies;
            model.addressList = addresses;
            model.contactList = contacts;
            model.user = user;
            return View(model);
        }
        public ActionResult Delete(Guid? Id)
        {
            if (Id == null) return RedirectToAction("Index", "Activity");
            if (Session["user"] == null) return RedirectToAction("Index", "Login");

            conn.Open();
            conn.Execute("DELETE FROM Activity WHERE Id = @Id", new Activity() { Id = Id});
            conn.Close();

            return Redirect("/Activity");
        }

        // POST: Activity
        [HttpPost]
        public ActionResult Add(Guid activityTypeId, Guid companyId, Guid addressId, Guid contactId, string title, string date, string status, string text)
        {
            if (Session["user"] == null) return RedirectToAction("Index", "Login");

            DateTime dateParse = PublicFunctions.stringToDateTimeEdit(date);
            
            conn.Open();
            conn.Execute("INSERT INTO Activity (ActivityTypeId, CompanyId, AddressId, ContactId, Title, Date, Status, Text) VALUES (@ActivityTypeId, @CompanyId, @AddressId, @ContactId, @Title, @Date, @Status, @Text)", new Activity() { ActivityTypeId = activityTypeId, CompanyId = companyId, AddressId = addressId, ContactId = contactId, Title= title, Date = dateParse, Status = Convert.ToInt32(status), Text = text });
            conn.Close();

            return Redirect("/Activity");
        }
        [HttpPost]
        public ActionResult Save(Guid activityTypeId, Guid companyId, Guid addressId, Guid contactId, string title, string date, string status, string text, Guid ActivityId)
        {
            if (Session["user"] == null) return RedirectToAction("Index", "Login");
          
            DateTime dateParse = PublicFunctions.stringToDateTimeEdit(date);

            conn.Open();
            
            Activity activity = conn.Query<Activity>("SELECT * FROM Activity WHERE Id=@Id", new Activity() { Id = ActivityId }).FirstOrDefault();

            DateTime? StatusUpdateDateTime = new DateTime();
            if (activity.Status != int.Parse(status)) StatusUpdateDateTime = DateTime.Now;
            else StatusUpdateDateTime = activity.StatusUpdateDateTime;

            conn.Execute("UPDATE Activity SET ActivityTypeId = @ActivityTypeId, CompanyId = @CompanyId, AddressId = @AddressId, ContactId = @ContactId, Title = @Title, Date = @Date, Status = @Status, Text = @Text, StatusUpdateDateTime = @StatusUpdateDateTime WHERE Id = @Id", new Activity() { Id = ActivityId, ActivityTypeId = activityTypeId, CompanyId = companyId, AddressId = addressId, ContactId = contactId, Title = title, Date = dateParse, Status = Convert.ToInt32(status), Text = text, StatusUpdateDateTime = StatusUpdateDateTime });
            
            conn.Close();
            
            return Redirect("/Activity/Detail/" + ActivityId);
        }

        // FUNCTİONS
        private List<Activity> editActivitiesAddDetails(List<Activity> activities)
        {
            conn.Open();

            foreach (var activity in activities)
            {
                ActivityType activityType = conn.Query<ActivityType>("SELECT * FROM ActivityType WHERE Id = @Id", new ActivityType() { Id = activity.ActivityTypeId }).FirstOrDefault();
                Company company = conn.Query<Company>("SELECT * FROM Company WHERE Id = @Id", new Company() { Id = activity.CompanyId }).FirstOrDefault();
                Address address = conn.Query<Address>("SELECT * FROM Address WHERE Id = @Id", new Address() { Id = activity.AddressId }).FirstOrDefault();
                Contact contact = conn.Query<Contact>("SELECT * FROM Contact WHERE Id = @Id", new Contact() { Id = activity.ContactId }).FirstOrDefault();

                if (activityType != null)
                    activity.ActivityType = activityType;
                else
                    activity.ActivityType = new ActivityType() { };

                if (address != null)
                    activity.Address = address;
                else
                    activity.Address = new Address() { };

                if (contact != null)
                    activity.Contact = contact;
                else
                    activity.Contact = new Contact() { };

                if (company != null)
                    activity.Company = company;
                else
                    activity.Company = new Company() { };
            }

            conn.Close();

            return activities;
        }
    }
}