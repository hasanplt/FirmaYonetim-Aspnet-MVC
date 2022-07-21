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
    public class ActivityTypeController : Controller
    {
        IDbConnection conn = Connection.conn;

        // GET: ActivityType
        public ActionResult Add()
        {
            if (Session["user"] == null) return RedirectToAction("Index", "Login");

            return View(new ViewModel()
            {
                user = PublicFunctions.getUser(conn, Session["user"].ToString())
            });
        }
        public ActionResult Detail(Guid? Id)
        {
            if (Session["user"] == null) return RedirectToAction("Index", "Login");
            if (!Id.HasValue) return Redirect("/Activity");

            User user = PublicFunctions.getUser(conn, Session["user"].ToString());

            conn.Open();
            ActivityType activityType = conn.Query<ActivityType>("SELECT * FROM ActivityType WHERE Id = @Id and CreatedByUserId = @CreatedByUserId", new ActivityType() { Id = Id, CreatedByUserId = user.Id }).FirstOrDefault();
            conn.Close();

            return View(new ViewModel()
            {
                activityType = activityType,
                user = PublicFunctions.getUser(conn, Session["user"].ToString())
            });
        }
        public ActionResult Delete(Guid? Id)
        {
            if (Session["user"] == null) return RedirectToAction("Index", "Login");
            if (!Id.HasValue) return Redirect("/Activity");

            User user = PublicFunctions.getUser(conn, Session["user"].ToString());

            conn.Open();
            conn.Execute("DELETE FROM ActivityType WHERE Id = @Id and CreatedByUserId = @CreatedByUserId", new ActivityType() { Id = Id, CreatedByUserId = user.Id });
            conn.Close();

            return Redirect("/Activity");
        }

        // POST: ActivityType
        [HttpPost]
        public ActionResult AddActivityType(string text)
        {
            if (Session["user"] == null) return RedirectToAction("Index", "Login");

            User user = PublicFunctions.getUser(conn, Session["user"].ToString());

            conn.Open();
            conn.Execute("INSERT INTO ActivityType (Text, CreatedByUserId) VALUES (@Text, CreatedByUserId)", new ActivityType() { Text = text, CreatedByUserId = user.Id });
            conn.Close();

            return Redirect("/Activity");
        }
        [HttpPost]
        public ActionResult Edit(string text, Guid Id)
        {
            if (Session["user"] == null) return RedirectToAction("Index", "Login");

            User user = PublicFunctions.getUser(conn, Session["user"].ToString());
            
            conn.Open();
            conn.Execute("UPDATE ActivityType SET Text = @Text WHERE Id=@Id and CreatedByUserId=@CreatedByUserId", new ActivityType() { Id = Id, Text = text, CreatedByUserId = user.Id });
            conn.Close();

            return Redirect("/ActivityType/Detail/" + Id);
        }

        // FUNCTİONS
    }
}