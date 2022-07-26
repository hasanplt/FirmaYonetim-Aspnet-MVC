﻿using Dapper;
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
    public class CompanyController : Controller
    {

        IDbConnection conn = Connection.conn;

        // GET: Company
        public ActionResult Index()
        {
            if (Session["user"] == null) return RedirectToAction("Index", "Login");

            conn.Open();

            User UserInfos = conn.Query<User>("SELECT * FROM [User] WHERE Email = @Email", new User() { Email = Session["user"].ToString() }).FirstOrDefault();
            List<Company> companies = conn.Query<Company>("SELECT * FROM [Company] WHERE CreatedByUserId = @CreatedByUserId and IsDelete = @IsDelete", new Company() { CreatedByUserId = (Guid)UserInfos.Id, IsDelete = false}).ToList();
            List<SharedCompany> sharedCompanies = conn.Query<SharedCompany>("SELECT * FROM SharedCompany WHERE SeeUserId = @SeeUserId", new SharedCompany() { SeeUserId = (Guid)UserInfos.Id}).ToList();
            foreach (var sharedCompany in sharedCompanies)
            {
                sharedCompany.company = conn.Query<Company>("SELECT * FROM Company WHERE Id = @Id", new Company() { Id = sharedCompany.CompanyId}).FirstOrDefault();
                sharedCompany.company.CreatedByUser = conn.Query<User>("SELECT * FROM [User] WHERE Id = @Id", new User() { Id = sharedCompany.company.CreatedByUserId }).FirstOrDefault();
            }
            foreach (var item in companies)
            {
                item.CreatedByUser = UserInfos;
            }

            conn.Close();

            return View(new ViewModel()
            {
                user = PublicFunctions.getUser(conn, Session["user"].ToString()),
                companyList = companies,
                sharedCompanyList = sharedCompanies
            });
        }
        public ActionResult Detail(Guid? Id)
        {
            if(Id == null) return Redirect("/");
            if (Session["user"] == null) return RedirectToAction("Index", "Login");

            User user = PublicFunctions.getUser(conn, Session["user"].ToString());

            conn.Open();

            Company company = conn.Query<Company>("SELECT * FROM [Company] WHERE Id = @Id and IsDelete = @IsDelete and CreatedByUserId = @CreatedByUserId", new Company() { Id = Id, IsDelete = false, CreatedByUserId = (Guid)user.Id }).FirstOrDefault();

            User UserInfos = conn.Query<User>("SELECT * FROM [User] WHERE Id = @Id", new User() { Id = company.CreatedByUserId }).FirstOrDefault();
            List<Address> AddressList = conn.Query<Address>("SELECT * FROM [Address] WHERE CompanyId = @CompanyId and IsDelete = @IsDelete", new Address() { CompanyId = (Guid)company.Id, IsDelete = false}).ToList();
            List<Activity> ActivityList = conn.Query<Activity>("SELECT * FROM [Activity] WHERE CompanyId = @CompanyId", new Activity() { CompanyId = (Guid)company.Id}).ToList();
            List<User> UserList = conn.Query<User>("SELECT * FROM [User] WHERE Id != @Id", UserInfos).ToList();
            List<SharedCompany> sharedCompanies = conn.Query<SharedCompany>("SELECT * FROM SharedCompany WHERE CompanyId = @CompanyId", new SharedCompany() { CompanyId = (Guid)company.Id }).ToList();

            foreach (var sharedCompany in sharedCompanies)
            {
                User sharedCompanyUser = conn.Query<User>("SELECT * FROM [User] WHERE Id = @Id", new User(){ Id = sharedCompany.SeeUserId }).FirstOrDefault();
                sharedCompany.seeUser = sharedCompanyUser;
                UserList = (from userDetail in UserList where userDetail.Id != sharedCompanyUser.Id select userDetail).ToList();
            }

            conn.Close();

            if (company == null) return RedirectToAction("Index", "Company");

            ActivityList = activitiesAddDetails(ActivityList);
            List<Contact> contactList = addreessesContactList(AddressList);
            company = companyUpdateDetails(company, AddressList, contactList,ActivityList, UserInfos);

            return View(new ViewModel()
            {
                company = company,
                userList = UserList,
                sharedCompanyList = sharedCompanies,
                user = user
            });
        }
        public ActionResult Delete(Guid? Id)
        {
            if (Id == null) return Redirect("/");
            if (Session["user"] == null) return RedirectToAction("Index", "Login");

            User user = PublicFunctions.getUser(conn, Session["user"].ToString());

            conn.Open();
            
            conn.Execute("UPDATE Company SET IsDelete = @IsDelete, IsDeleteDateTime = @IsDeleteDateTime WHERE Id = @Id and CreatedByUserId = @CreatedByUserId", new Company() { Id=Id, IsDelete=true, IsDeleteDateTime = DateTime.Now, CreatedByUserId = (Guid)user.Id });
            Company company = conn.Query<Company>("SELECT * FROM Company WHERE Id = @Id", new Company() { Id = Id }).FirstOrDefault();

            conn.Close();
            
            deleteConnectToCompanyDetails(company);

            return RedirectToAction("Index", "Company");
        }

        // POST: Company
        [HttpPost]
        public ActionResult Add(string name)
        {
            if (Session["user"] == null) return RedirectToAction("Index", "Login");
            
            User user = PublicFunctions.getUser(conn, Session["user"].ToString());
            
            conn.Open();
            conn.Execute("INSERT INTO Company (Name, CreatedByUserId) VALUES (@Name, @CreatedByUserId)", new Company() { Name = name, CreatedByUserId = (Guid)user.Id });
            conn.Close();

            return RedirectToAction("Index", "Company");
        }
        [HttpPost]
        public ActionResult Save(string name, Guid id, Guid userId)
        {
            if (Session["user"] == null) return RedirectToAction("Index", "Login");

            User user = PublicFunctions.getUser(conn, Session["user"].ToString());

            conn.Open();
            conn.Execute("UPDATE Company SET Name = @Name, UpdateDateTime = @UpdateDateTime, UpdatedByUserId = @UpdatedByUserId WHERE Id = @Id and CreatedByUserId = @CreatedByUserId", new Company() { Name=name, UpdateDateTime = DateTime.Now, UpdatedByUserId = userId, Id=id, CreatedByUserId = (Guid)user.Id });
            conn.Close();

            return Redirect("/Company/Detail/" + id);
        }

        // FUNCTİONS
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
        private void deleteConnectToCompanyDetails(Company company)
        {
            conn.Open();

            // Delete Addresses and Connections
            List<Address> addresses = conn.Query<Address>("SELECT * FROM Address WHERE CompanyId = @CompanyId", new Address() { CompanyId = (Guid)company.Id }).ToList();
            foreach (var address in addresses)
            {
                conn.Execute("UPDATE Address SET IsDelete = @IsDelete, IsDeleteDateTime = @IsDeleteDateTime WHERE Id = @Id", new Address() { Id = address.Id, IsDelete = true, IsDeleteDateTime = DateTime.Now });
                
                // Delete Contacts Connect to Address
                List<Contact> contacts = conn.Query<Contact>("SELECT * FROM Contact WHERE AddressId = @AddressId", new Contact() { AddressId = (Guid)address.Id }).ToList();
                foreach (var contact in contacts)
                {
                    conn.Execute("UPDATE Contact SET IsDelete = @IsDelete, IsDeleteDateTime = @IsDeleteDateTime WHERE Id = @Id", new Contact() { Id = contact.Id, IsDelete = true, IsDeleteDateTime = DateTime.Now });
                }
            }
            
            // Delete Activities
            List<Activity> activities = conn.Query<Activity>("SELECT * FROM Activity WHERE CompanyId = @CompanyId", new Activity() { CompanyId = (Guid)company.Id }).ToList();
            foreach (var activity in activities)
            {
                conn.Execute("DELETE FROM Activity WHERE Id = @Id", new Activity() { Id = activity.Id });
            }

            conn.Close();
        }
    }
}