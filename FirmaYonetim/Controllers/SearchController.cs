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
    public class SearchController : Controller
    {
        IDbConnection conn = Connection.conn;

        // GET: Search
        public ActionResult Index(string searchKeyword)
        {
            if (searchKeyword == "" || searchKeyword == null) return Redirect("/");
            if (Session["user"] == null) return RedirectToAction("Index", "Login");

            searchKeyword = searchKeyword.ToLower();
            User user = PublicFunctions.getUser(conn, Session["User"].ToString());

            conn.Open();

            List<Company> companies = conn.Query<Company>("SELECT * FROM [Company] WHERE IsDelete = @IsDelete and CreatedByUserId = @CreatedByUserId", new Company() { IsDelete = false, CreatedByUserId = (Guid)user.Id }).ToList();
            List<Address> AddressList = conn.Query<Address>("SELECT * FROM [Address] WHERE IsDelete = @IsDelete and CreatedByUserId = @CreatedByUserId", new Address() {  IsDelete = false, CreatedByUserId = (Guid)user.Id }).ToList();
            List<Activity> ActivityList = conn.Query<Activity>("SELECT * FROM [Activity] WHERE UserId = @UserId", new Activity() { UserId = (Guid)user.Id }).ToList();
            List<Contact> contactList = conn.Query<Contact>("SELECT * FROM [Contact] WHERE IsDelete = @IsDelete and CreatedByUserId = @CreatedByUserId", new Contact() { IsDelete = false, CreatedByUserId = (Guid)user.Id }).ToList();

            ActivityList = addDetailsToActivityList(ActivityList);

            conn.Close();

            // Company add detail of user
            foreach (var company in companies)
            {
                User UserInfos = conn.Query<User>("SELECT * FROM [User] WHERE Id = @Id", new User() { Id = company.CreatedByUserId }).FirstOrDefault();
                company.CreatedByUser = UserInfos;
            }

            // Filter Datas
            List<Company> companiesFilter = companies.Where(company => company.Name.ToLower().Contains(searchKeyword) || company.CreateDateTime.ToString().ToLower().Contains(searchKeyword)).ToList();
            List<Address> addressesFilter = AddressList.Where(address => address.Name.ToLower().Contains(searchKeyword) || address.Province.ToLower().Contains(searchKeyword) || address.District.ToLower().Contains(searchKeyword) || address.AddressDetail.ToLower().Contains(searchKeyword) || address.Phone1.ToLower().Contains(searchKeyword) || address.Phone2.ToLower().Contains(searchKeyword) || address.Phone3.ToLower().Contains(searchKeyword) || address.Email.ToLower().Contains(searchKeyword) || address.CreatedDateTime.ToString().ToLower().Contains(searchKeyword)).ToList();
            List<Activity> activitiesFilter = ActivityList.Where(activity => activity.Title.ToLower().Contains(searchKeyword) || activity.Text.ToLower().Contains(searchKeyword) || activity.Date.ToString().ToLower().Contains(searchKeyword) || activity.CreatedDateTime.ToString().ToLower().Contains(searchKeyword) || activity.ActivityType.Text.ToLower().Contains(searchKeyword)).ToList();
            List<Contact> contactsFilter = contactList.Where(contact => contact.Name.ToLower().Contains(searchKeyword) || contact.Surname.ToLower().Contains(searchKeyword) || contact.Email.ToLower().Contains(searchKeyword) || contact.GSM.ToLower().Contains(searchKeyword) || contact.Title.ToLower().Contains(searchKeyword) || contact.LandPhone.ToLower().Contains(searchKeyword) || contact.LandPhoneInternal.ToLower().Contains(searchKeyword) || contact.CreatedDateTime.ToString().ToLower().Contains(searchKeyword)).ToList();

            return View(new ViewModel() { 
                companyList = companiesFilter,
                addressList = addressesFilter,
                activityList = activitiesFilter,
                contactList = contactsFilter,
                user = user,
            });
        }

        // FUNCTİONS
        private List<Activity> addDetailsToActivityList(List<Activity> activityList)
        {
            foreach (var activity in activityList)
            {
                ActivityType ActivityType = conn.Query<ActivityType>("SELECT * FROM [ActivityType] WHERE Id = @Id", new ActivityType() { Id = activity.ActivityTypeId }).FirstOrDefault();
                Company ActivityCompany = conn.Query<Company>("SELECT * FROM [Company] WHERE Id = @Id", new Company() { Id = activity.CompanyId }).FirstOrDefault();
                Address ActivityAddress = conn.Query<Address>("SELECT * FROM [Address] WHERE Id = @Id", new Address() { Id = activity.AddressId }).FirstOrDefault();
                Contact ActivityContact = conn.Query<Contact>("SELECT * FROM [Contact] WHERE Id = @Id", new Contact() { Id = activity.ContactId }).FirstOrDefault();
                activity.ActivityType = ActivityType;
                activity.Company = ActivityCompany;
                activity.Address = ActivityAddress;
                activity.Contact = ActivityContact;
            }
            return activityList;
        }
    }
}