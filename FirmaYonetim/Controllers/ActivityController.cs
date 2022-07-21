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

            User user = PublicFunctions.getUser(conn, Session["user"].ToString());

            conn.Open();

            List<Activity> activities = conn.Query<Activity>("SELECT * FROM Activity WHERE UserId = @UserId", new Activity() { UserId = (Guid)user.Id}).ToList();
            List<ActivityType> activityTypes = conn.Query<ActivityType>("SELECT * FROM ActivityType WHERE IsDelete = @IsDelete and CreatedByUserId=@CreatedByUserId", new ActivityType() { IsDelete = false, CreatedByUserId = user.Id }).ToList();

            conn.Close();

            activities = editActivitiesAddDetails(activities);

            return View(new ViewModel()
            {
                activityTypeList = activityTypes,
                activityList = activities,
                user = user
            });
        }
        public ActionResult Add()
        {
            if (Session["user"] == null) return RedirectToAction("Index", "Login");
            User user = PublicFunctions.getUser(conn, Session["user"].ToString());

            conn.Open();
            
            List<ActivityType> activityTypes = conn.Query<ActivityType>("SELECT * FROM ActivityType WHERE IsDelete = @IsDelete and CreatedByUserId = @CreatedByUserId", new ActivityType() { IsDelete = false, CreatedByUserId = user.Id }).ToList();
            List<Company> companies = conn.Query<Company>("SELECT * FROM Company WHERE IsDelete = @IsDelete and CreatedByUserId = @CreatedByUserId", new Company() { IsDelete = false, CreatedByUserId = (Guid)user.Id }).ToList();
            
            conn.Close();

            ViewModel model = new ViewModel();
            model.activityTypeList = activityTypes;
            model.companyList = companies;
            model.user = user;
            return View(new ViewModel()
            {
                activityTypeList = activityTypes,
                companyList = companies,
                user = user
            });
        }
        public ActionResult Detail(Guid? Id)
        {
            if (Id == null) return RedirectToAction("Index", "Home");
            if (Session["user"] == null) return RedirectToAction("Index", "Login");

            User user = PublicFunctions.getUser(conn, Session["user"].ToString());

            conn.Open();
            
            List<ActivityType> activityTypes = conn.Query<ActivityType>("SELECT * FROM ActivityType WHERE IsDelete = @IsDelete and CreatedByUserId = @CreatedByUserId", new ActivityType() { IsDelete = false }).ToList();
            List<Company> companies = conn.Query<Company>("SELECT * FROM Company WHERE IsDelete = @IsDelete and CreatedByUserId = @CreatedByUserId", new Company() { IsDelete = false, CreatedByUserId = (Guid)user.Id }).ToList();
            Activity activity = conn.Query<Activity>("SELECT * FROM Activity WHERE Id = @Id and UserId = @UserId", new Activity() { Id = Id, UserId = (Guid)user.Id }).FirstOrDefault();
            List<Address> addresses = conn.Query<Address>("SELECT * FROM Address WHERE IsDelete = @IsDelete and CompanyId = @CompanyId", new Address() { IsDelete = false, CompanyId = activity.CompanyId }).ToList();
            List<Contact> contacts = conn.Query<Contact>("SELECT * FROM Contact WHERE IsDelete = @IsDelete and AddressId = @AddressId", new Contact() { IsDelete = false, AddressId = activity.AddressId }).ToList();
            
            conn.Close();

            if(activity == null)
            {
                return RedirectToAction("Index", "Home");
            }

            activity.editDateTime = PublicFunctions.dateTimeToStringEdit((DateTime)activity.Date);

            return View(new ViewModel()
            {
                activityTypeList = activityTypes,
                activity = activity,
                companyList = companies,
                addressList = addresses,
                contactList = contacts,
                user = user
            });
        }
        public ActionResult Delete(Guid? Id)
        {
            if (Id == null) return RedirectToAction("Index", "Activity");
            if (Session["user"] == null) return RedirectToAction("Index", "Login");

            User user = PublicFunctions.getUser(conn, Session["user"].ToString());

            conn.Open();
            conn.Execute("DELETE FROM Activity WHERE Id = @Id and UserId = @UserId", new Activity() { Id = Id, UserId = (Guid)user.Id});
            conn.Close();

            return Redirect("/Activity");
        }

        // POST: Activity
        [HttpPost]
        public ActionResult Add(Guid activityTypeId, Guid companyId, Guid addressId, Guid contactId, string title, string date, string status, string text)
        {
            if (Session["user"] == null) return RedirectToAction("Index", "Login");


            DateTime dateParse = PublicFunctions.stringToDateTimeEdit(date);
            User user = PublicFunctions.getUser(conn, Session["user"].ToString());

            conn.Open();
            conn.Execute("INSERT INTO Activity (ActivityTypeId, CompanyId, AddressId, ContactId, Title, Date, Status, Text, UserId) VALUES (@ActivityTypeId, @CompanyId, @AddressId, @ContactId, @Title, @Date, @Status, @Text, @UserId)", new Activity() { ActivityTypeId = activityTypeId, CompanyId = companyId, AddressId = addressId, ContactId = contactId, Title= title, Date = dateParse, Status = Convert.ToInt32(status), Text = text, UserId = (Guid)user.Id});
            conn.Close();

            return Redirect("/Activity");
        }
        [HttpPost]
        public ActionResult Save(Guid activityTypeId, Guid companyId, Guid addressId, Guid contactId, string title, string date, string status, string text, Guid ActivityId)
        {
            if (Session["user"] == null) return RedirectToAction("Index", "Login");
          
            DateTime dateParse = PublicFunctions.stringToDateTimeEdit(date);
            User user = PublicFunctions.getUser(conn, Session["user"].ToString());

            conn.Open();
            
            Activity activity = conn.Query<Activity>("SELECT * FROM Activity WHERE Id=@Id", new Activity() { Id = ActivityId }).FirstOrDefault();

            DateTime? StatusUpdateDateTime = new DateTime();
            if (activity.Status != int.Parse(status)) StatusUpdateDateTime = DateTime.Now;
            else StatusUpdateDateTime = activity.StatusUpdateDateTime;

            conn.Execute("UPDATE Activity SET ActivityTypeId = @ActivityTypeId, CompanyId = @CompanyId, AddressId = @AddressId, ContactId = @ContactId, Title = @Title, Date = @Date, Status = @Status, Text = @Text, StatusUpdateDateTime = @StatusUpdateDateTime WHERE Id = @Id and UserId=@UserId", new Activity() { Id = ActivityId, ActivityTypeId = activityTypeId, CompanyId = companyId, AddressId = addressId, ContactId = contactId, Title = title, Date = dateParse, Status = Convert.ToInt32(status), Text = text, StatusUpdateDateTime = StatusUpdateDateTime, UserId = (Guid)user.Id });
            
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