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
    public class ContactController : Controller
    {
        IDbConnection conn = Connection.conn;
       
        // GET: Contact
        public ActionResult Index()
        {
            if (Session["user"] == null) return RedirectToAction("Index", "Login");
            
            conn.Open();
            List<Contact> ContactList = conn.Query<Contact>("SELECT * FROM Contact WHERE IsDelete = @IsDelete", new Contact() { IsDelete = false }).ToList();
            conn.Close();

            ContactList = contactListAddDetails(ContactList);

            return View(new ViewModel()
            {
                contactList = ContactList,
                user = PublicFunctions.getUser(conn, Session["user"].ToString())
            });
        }
        public ActionResult Add()
        {
            if (Session["user"] == null) return RedirectToAction("Index", "Login");

            conn.Open();
            List<Address> address = conn.Query<Address>("SELECT * FROM Address WHERE IsDelete = @IsDelete", new Address() { IsDelete = false }).ToList();
            conn.Close();

            return View(new ViewModel()
            {
                user = PublicFunctions.getUser(conn, Session["user"].ToString()),
                addressList = address
            });
        }
        public ActionResult Detail(Guid? Id)
        {
            if (Id == null) return Redirect("/");
            if (Session["user"] == null) return RedirectToAction("Index", "Login");

            conn.Open();

            Contact contactDetail = conn.Query<Contact>("SELECT * FROM Contact WHERE Id = @Id and IsDelete = @IsDelete", new Contact() { Id = Id, IsDelete = false }).FirstOrDefault();
            List<Address> adressList = conn.Query<Address>("SELECT * FROM Address WHERE IsDelete = @IsDelete", new Address() { IsDelete = false }).ToList();
            List<Activity> activityList = conn.Query<Activity>("SELECT * FROM Activity WHERE ContactId = @ContactId", new Activity() { ContactId = (Guid)contactDetail.Id }).ToList();

            conn.Close();

            contactDetail.ActivityList = activityListAddDetails(activityList);

            return View(new ViewModel()
            {
                addressList = adressList,
                contact = contactDetail,
                user = PublicFunctions.getUser(conn, Session["user"].ToString())
            });
        }
        public ActionResult Delete(Guid? Id)
        {
            if (Id == null) return Redirect("/");
            if (Session["user"] == null) return RedirectToAction("Index", "Login");

            conn.Open();
            
            conn.Execute("UPDATE Contact SET IsDelete=@IsDelete, IsDeleteDateTime = @IsDeleteDateTime WHERE Id = @Id", new Contact { Id = Id, IsDelete = true, IsDeleteDateTime = DateTime.Now });            
            Contact contact = conn.Query<Contact>("SELECT * FROM Contact WHERE Id = @Id", new Contact { Id = Id}).FirstOrDefault();

            conn.Close();

            deleteContactDetails(contact);

            return Redirect("/Contact");
        }

        // POST: Contact
        [HttpPost]
        public ActionResult Save(string name, string surname, string title, string email, Guid addressId, string gsm, string landPhone, string landPhoneInternal, Guid userId)
        {
            if (Session["user"] == null) return RedirectToAction("Index", "Login");
            
            conn.Open();
            conn.Execute("UPDATE Contact SET Name = @Name, Surname = @Surname, Title = @Title, Email = @Email, AddressId = @AddressId, GSM = @GSM, LandPhone = @LandPhone, LandPhoneInternal = @LandPhoneInternal WHERE Id = @Id", new Contact() { Id = userId, Name = name, Surname = surname, Title = title, Email = email, AddressId = addressId, GSM = gsm, LandPhone = landPhone, LandPhoneInternal = landPhoneInternal });
            conn.Close();
            
            return Redirect("/Contact/Detail/" + userId);
        }
        [HttpPost]
        public ActionResult Add(string name, string surname, string title, string email, Guid addressId, string gsm, string landPhone, string landPhoneInternal, Guid createdByUserId)
        {
            if (Session["user"] == null) return RedirectToAction("Index", "Login");
           
            conn.Open();
            conn.Execute("INSERT INTO Contact (Name,Surname, Title, Email, AddressId, GSM, LandPhone, LandPhoneInternal, CreatedByUserId) VALUES (@Name, @Surname, @Title, @Email, @AddressId, @GSM, @LandPhone, @LandPhoneInternal, @CreatedByUserId)", new Contact() { Name = name, Surname = surname, Title = title, Email = email, AddressId = addressId, GSM = gsm, LandPhone = landPhone, LandPhoneInternal = landPhoneInternal, CreatedByUserId = createdByUserId });
            conn.Close();
            
            return Redirect("/Contact");
        }

        // FUNCTİONS
        private List<Contact> contactListAddDetails(List<Contact> contactList)
        {
            conn.Open();
            foreach (var contact in contactList)
            {
                Address address = conn.Query<Address>("SELECT * FROM Address WHERE IsDelete = @IsDelete and Id = @Id", new Address() { IsDelete = false, Id = contact.AddressId }).FirstOrDefault();
                contact.Address = address;
            }
            conn.Close();

            return contactList;
        }
        private List<Activity> activityListAddDetails(List<Activity> activityList)
        {
            conn.Open();
            foreach (var activity in activityList)
            {
                ActivityType ActivityType = conn.Query<ActivityType>("SELECT * FROM [ActivityType] WHERE Id = @Id", new ActivityType() { Id = activity.ActivityTypeId }).FirstOrDefault();
                Company ActivityCompany = conn.Query<Company>("SELECT * FROM [Company] WHERE Id = @Id", new Company() { Id = activity.CompanyId }).FirstOrDefault();
                Address ActivityAddress = conn.Query<Address>("SELECT * FROM [Address] WHERE Id = @Id", new Address() { Id = activity.AddressId }).FirstOrDefault();
                activity.ActivityType = ActivityType;
                activity.Company = ActivityCompany;
                activity.Address = ActivityAddress;
            }
            conn.Close();

            return activityList;
        }
        private void deleteContactDetails(Contact contact)
        {
            conn.Open();
            List<Activity> activities = conn.Query<Activity>("SELECT * FROM Activity WHERE ContactId = @ContactId", new Activity() { ContactId = (Guid)contact.Id }).ToList();
            foreach (var activity in activities)
            {
                conn.Execute("DELETE FROM Activity WHERE Id = @Id", new Activity() { Id = activity.Id });
            }
            conn.Close();
        }
    }
}