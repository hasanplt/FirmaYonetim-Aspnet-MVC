using Dapper;
using FirmaYonetim.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FirmaYonetim.Controllers
{
    public class SharedCompanyController : Controller
    {
        IDbConnection conn = Connection.conn;
        // GET: SharedCompany
        public ActionResult Delete(Guid SharedCompanyId, Guid companyId)
        {
            if (Session["user"] == null) return Redirect("/Login/Index");

            User user = PublicFunctions.getUser(conn, Session["user"].ToString());

            conn.Open();
            conn.Execute("DELETE FROM SharedCompany WHERE Id=@Id and WhoSharedUserId = @WhoSharedUserId", new SharedCompany() { Id = SharedCompanyId, WhoSharedUserId = (Guid)user.Id });
            conn.Close();

            return Redirect("/Company/Detail/" + companyId);
        }
        public ActionResult Detail(Guid Id)
        {
            if (Session["user"] == null) return Redirect("/Login/Index");

            conn.Open();

            Company company = conn.Query<Company>("SELECT * FROM Company WHERE Id = @Id and IsDelete = @IsDelete", new Company() { Id = Id, IsDelete = false }).FirstOrDefault();

            User UserInfos = conn.Query<User>("SELECT * FROM [User] WHERE Id = @Id", new User() { Id = company.CreatedByUserId }).FirstOrDefault();
            List<Address> AddressList = conn.Query<Address>("SELECT * FROM [Address] WHERE CompanyId = @CompanyId and IsDelete = @IsDelete", new Address() { CompanyId = (Guid)company.Id, IsDelete = false }).ToList();
            List<Activity> ActivityList = conn.Query<Activity>("SELECT * FROM [Activity] WHERE CompanyId = @CompanyId", new Activity() { CompanyId = (Guid)company.Id }).ToList();
            List<User> UserList = conn.Query<User>("SELECT * FROM [User] WHERE Id != @Id", UserInfos).ToList();
            List<SharedCompany> sharedCompanies = conn.Query<SharedCompany>("SELECT * FROM SharedCompany WHERE CompanyId = @CompanyId", new SharedCompany() { CompanyId = (Guid)company.Id }).ToList();

            foreach (var sharedCompany in sharedCompanies)
            {
                User sharedCompanyUser = conn.Query<User>("SELECT * FROM [User] WHERE Id = @Id", new User() { Id = sharedCompany.SeeUserId }).FirstOrDefault();
                sharedCompany.seeUser = sharedCompanyUser;
                UserList = (from userDetail in UserList where userDetail.Id != sharedCompanyUser.Id select userDetail).ToList();
            }

            conn.Close();

            if (company == null) return RedirectToAction("Index", "Company");

            ActivityList = activitiesAddDetails(ActivityList);
            List<Contact> contactList = addreessesContactList(AddressList);
            company = companyUpdateDetails(company, AddressList, contactList, ActivityList, UserInfos);

            return View(new ViewModel()
            {
                company = company,
                user = PublicFunctions.getUser(conn, Session["user"].ToString())
            });
        }
        public ActionResult AddressDetail(Guid? Id)
        {
            if (Id == null) return RedirectToAction("Index", "Home");
            if (Session["user"] == null) return RedirectToAction("Index", "Login");

            conn.Open();
            Address address = conn.Query<Address>("SELECT * FROM Address WHERE Id = @Id", new Address() { Id = Id }).FirstOrDefault();
            conn.Close();

            return View(new ViewModel()
            {
                address = address,
                user = PublicFunctions.getUser(conn, Session["user"].ToString())
            });
        }
        public ActionResult ContactDetail(Guid Id)
        {
            if (Id == null) return Redirect("/");
            if (Session["user"] == null) return RedirectToAction("Index", "Login");

            User user = PublicFunctions.getUser(conn, Session["User"].ToString());

            conn.Open();

            Contact contactDetail = conn.Query<Contact>("SELECT * FROM Contact WHERE Id = @Id and IsDelete = @IsDelete", new Contact() { Id = Id, IsDelete = false}).FirstOrDefault();
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
        public ActionResult ActivityDetail(Guid Id)
        {
            if (Id == null) return RedirectToAction("Index", "Home");
            if (Session["user"] == null) return RedirectToAction("Index", "Login");

            User user = PublicFunctions.getUser(conn, Session["user"].ToString());

            conn.Open();

            List<ActivityType> activityTypes = conn.Query<ActivityType>("SELECT * FROM ActivityType WHERE IsDelete = @IsDelete", new ActivityType() { IsDelete = false }).ToList();
            List<Company> companies = conn.Query<Company>("SELECT * FROM Company WHERE IsDelete = @IsDelete", new Company() { IsDelete = false }).ToList();
            Activity activity = conn.Query<Activity>("SELECT * FROM Activity WHERE Id = @Id", new Activity() { Id = Id }).FirstOrDefault();
            List<Address> addresses = conn.Query<Address>("SELECT * FROM Address WHERE IsDelete = @IsDelete and CompanyId = @CompanyId", new Address() { IsDelete = false, CompanyId = activity.CompanyId }).ToList();
            List<Contact> contacts = conn.Query<Contact>("SELECT * FROM Contact WHERE IsDelete = @IsDelete and AddressId = @AddressId", new Contact() { IsDelete = false, AddressId = activity.AddressId }).ToList();

            conn.Close();

            if (activity == null)
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

        // POST: SharedCompany
        [HttpPost]
        public ActionResult Add(Guid SharedUserId, Guid userId, Guid companyId)
        {
            if (Session["user"] == null) return Redirect("/Login/Index");

            conn.Open();
            conn.Execute("INSERT INTO SharedCompany (CompanyId,SeeUserId,WhoSharedUserId) VALUES (@CompanyId,@SeeUserId,@WhoSharedUserId)", new SharedCompany() { CompanyId = companyId, SeeUserId = SharedUserId, WhoSharedUserId = userId });
            conn.Close();

            return Redirect("/Company/Detail/" + companyId);
        }

        // Functions
        private List<Contact> addreessesContactList(List<Address> addressList)
        {
            List<Contact> contactList = new List<Contact>();

            conn.Open();

            foreach (var address in addressList)
            {
                List<Contact> addressContactList = conn.Query<Contact>("SELECT * FROM [Contact] WHERE AddressId = @AddressId and IsDelete = @IsDelete", new Contact() { AddressId = (Guid)address.Id, IsDelete = false }).ToList();
                contactList.AddRange(addressContactList);
            }

            conn.Close();

            return contactList;
        }
        private List<Activity> activitiesAddDetails(List<Activity> activityList)
        {
            conn.Open();
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
            conn.Close();
            return activityList;
        }
        private Company companyUpdateDetails(Company company, List<Address> AddressList, List<Contact> contactList, List<Activity> ActivityList, User user)
        {
            company.CreatedByUser = user;

            conn.Open();

            if (company.UpdatedByUserId != new Guid())
                company.UpdatedByUser = conn.Query<User>("SELECT * FROM [User] WHERE Id = @Id", new User() { Id = company.UpdatedByUserId }).FirstOrDefault();
            else
                company.UpdatedByUser = new User() { Name = "", Surname = "" };

            if (AddressList.Count != 0)
                company.AddressList = AddressList;
            else
                company.AddressList = new List<Address>();

            if (contactList.Count != 0)
                company.ContactList = contactList;
            else
                company.ContactList = new List<Contact>();

            if (ActivityList.Count != 0)
                company.ActivityList = ActivityList;
            else
                company.ActivityList = new List<Activity>();

            conn.Close();

            return company;
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
    }
}