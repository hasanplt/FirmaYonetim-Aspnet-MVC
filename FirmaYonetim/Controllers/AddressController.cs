using Dapper;
using FirmaYonetim.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace FirmaYonetim.Controllers
{
    public class AddressController : Controller
    {
        IDbConnection conn = Connection.conn;

        // GET: Address
        public ActionResult Add(string name, string province, string district, string phone1, string phone2, string phone3, string email, string address, Guid companyId)
        {
            if (Session["user"] == null) return RedirectToAction("Index", "Login");

            User user = PublicFunctions.getUser(conn, Session["user"].ToString());

            conn.Open();
            conn.Execute("INSERT INTO Address (Name, Province, District, Phone1, Phone2, Phone3, Email, AddressDetail, CompanyId, CreatedByUserId) VALUES (@Name, @Province, @District, @Phone1, @Phone2, @Phone3, @Email, @AddressDetail, @CompanyId, @CreatedByUserId)", new Address() { Name = name, Province= province, District = district, Phone1 = phone1, Phone2 = phone2, Phone3 = phone3, Email = email, AddressDetail = address, CompanyId = companyId, CreatedByUserId = (Guid)user.Id });
            conn.Close();
            
            return Redirect("/Company/Detail/" + companyId);
        }
        public ActionResult Detail(Guid? Id)
        {
            if (Id == null) return RedirectToAction("Index", "Home");
            if (Session["user"] == null) return RedirectToAction("Index", "Login");

            conn.Open();
            Address address = conn.Query<Address>("SELECT * FROM Address WHERE Id = @Id", new Address() { Id = Id}).FirstOrDefault();
            conn.Close();

            return View(new ViewModel()
            {
                address = address,
                user = PublicFunctions.getUser(conn, Session["user"].ToString())
            });
        }
        public ActionResult Delete(Guid? Id)
        {
            if (Id == null) return RedirectToAction("Index", "Home");
            if (Session["user"] == null) return RedirectToAction("Index", "Login");
            
            conn.Open();
            
            conn.Execute("UPDATE Address SET IsDelete = @IsDelete, IsDeleteDateTime = @IsDeleteDateTime WHERE Id = @Id", new Address() { Id= Id, IsDelete = true, IsDeleteDateTime = DateTime.Now });
            
            Address address = conn.Query<Address>("SELECT * FROM Address WHERE Id = @Id", new Address() { Id = Id }).FirstOrDefault();
            List<Contact> contacts = conn.Query<Contact>("SELECT * FROM Contact WHERE AddressId = @AddressId", new Contact() { AddressId = (Guid)address.Id }).ToList();
            List<Activity> activities = conn.Query<Activity>("SELECT * FROM Activity WHERE AddressId = @AddressId", new Activity() { AddressId = (Guid)address.Id }).ToList();

            conn.Close();

            deleteContactsAndActivities(contacts, activities);

            return Redirect("/Company/Detail/" + address.CompanyId);
        }

        // API: Address
        public ActionResult FullAddressAPI(Guid Id)
        {
            conn.Open();
            List<Address> addresses = conn.Query<Address>("SELECT Id, Name FROM Address WHERE IsDelete = @IsDelete and CompanyId = @CompanyId", new Address() { IsDelete = false, CompanyId = Id }).ToList();
            conn.Close(); 
            
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(addresses);
            
            Response.Write(json);
            return View();
        }
        public ActionResult FullContactAPI(Guid Id)
        {
            conn.Open();
            List<Contact> contacts = conn.Query<Contact>("SELECT Id, Name, Surname FROM Contact WHERE IsDelete = @IsDelete and AddressId = @AddressId", new Contact() { IsDelete = false, AddressId = Id }).ToList();
            conn.Close();
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(contacts);
            Response.Write(json);
            return View();
        }

        // POST: Address
        [HttpPost]
        public ActionResult Save(string name, string province, string district, string phone1, string phone2, string phone3, string email, string address, Guid Id)
        {
            if (Session["user"] == null) return RedirectToAction("Index", "Login");

            User user = PublicFunctions.getUser(conn, Session["user"].ToString());

            conn.Open();
            conn.Execute("UPDATE Address SET Name = @Name, Province = @Province, District = @District, Phone1 = @Phone1, Phone2 = @Phone2, Phone3 = @Phone3, Email = @Email, AddressDetail = @AddressDetail, CreatedByUserId = @CreatedByUserId WHERE Id = @Id", new Address() { Id = Id, Name = name, Province = province, District = district, Phone1 = phone1, Phone2 = phone2, Phone3 = phone3, Email = email, AddressDetail = address, CreatedByUserId = (Guid)user.Id });
            conn.Close();

            return Redirect("/Address/Detail/" + Id);
        }

        // FUNCTİONS
        private void deleteContactsAndActivities(List<Contact> contacts, List<Activity> activities)
        {
            conn.Open();

            foreach (var contact in contacts)
            {
                conn.Execute("UPDATE Contact SET IsDelete = @IsDelete, IsDeleteDateTime = @IsDeleteDateTime WHERE Id = @Id", new Contact() { Id = contact.Id, IsDelete = true, IsDeleteDateTime = DateTime.Now });
            }
            foreach (var activity in activities)
            {
                conn.Execute("DELETE FROM Activity WHERE Id = @Id", new Activity() { Id = activity.Id });
            }

            conn.Close();
        }
    }
}