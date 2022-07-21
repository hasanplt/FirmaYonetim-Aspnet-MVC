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
    public class ToDoController : Controller
    {
        IDbConnection conn = Connection.conn;

        // GET: ToDo
        public ActionResult Edit(Guid? Id)
        {
            if (Session["user"] == null) return RedirectToAction("Index", "Login");
            if (Id == null) return Redirect("/");

            User user = PublicFunctions.getUser(conn, Session["user"].ToString());

            conn.Open();
            ToDoList toDo = conn.Query<ToDoList>("SELECT * FROM ToDoList WHERE Id=@Id and CreatedByUserId=@CreatedByUserId", new ToDoList() { Id = Id, CreatedByUserId = user.Id }).FirstOrDefault();
            conn.Close();

            if(toDo == null)
            {
                return Redirect("/");
            }

            toDo.EditDateTime = PublicFunctions.dateTimeToStringEdit(toDo.Date);
            
            return View(new ViewModel()
            {
                toDo = toDo,
                user = user
            });
        }
        public ActionResult Delete(Guid? Id)
        {
            if (Session["user"] == null) return RedirectToAction("Index", "Login");
            if (Id == null) return Redirect("/");

            User user = PublicFunctions.getUser(conn, Session["user"].ToString());

            conn.Open();
            conn.Execute("DELETE FROM ToDoList WHERE Id = @Id and CreatedByUserId=@CreatedByUserId", new ToDoList() { Id = Id, CreatedByUserId = user.Id });
            conn.Close();

            return Redirect("/");
        }

        // POST: ToDo
        [HttpPost]
        public ActionResult Edit(string text, string date, Guid Id)
        {
            if (Session["user"] == null) return RedirectToAction("Index", "Login");

            DateTime dateParse = PublicFunctions.stringToDateTimeEdit(date);
            User user = PublicFunctions.getUser(conn, Session["user"].ToString());

            conn.Open();
            conn.Execute("UPDATE ToDoList SET Text = @Text, Date = @Date WHERE Id=@Id and CreatedByUserId = @CreatedByUserId", new ToDoList() { Id = Id, Text = text, Date = dateParse, CreatedByUserId = user.Id });
            conn.Close();
            
            return Redirect("/ToDo/Edit/" + Id);
        }
        [HttpPost]
        public ActionResult Add(string text, string date)
        {
            if (Session["user"] == null) return RedirectToAction("Index", "Login");
            DateTime dateParse = PublicFunctions.stringToDateTimeEdit(date);

            User user = PublicFunctions.getUser(conn, Session["user"].ToString());

            conn.Open();
            conn.Execute("INSERT INTO ToDoList (Text, Date, CreatedByUserId) VALUES (@Text, @Date, @CreatedByUserId)", new ToDoList() { Text = text, Date = dateParse, CreatedByUserId = user.Id });
            conn.Close();

            return Redirect("/");
        }

        // API: ToDo
        public ActionResult EditStatus(Guid? Id)
        {
            if (Id == null) return View();
            if (Session["user"] == null) return RedirectToAction("Index", "Login");

            User user = PublicFunctions.getUser(conn, Session["user"].ToString());

            conn.Open();

            ToDoList toDo = conn.Query<ToDoList>("SELECT * FROM ToDoList WHERE Id = @Id and CreatedByUserId = @CreatedByUserId", new ToDoList() { Id = Id, CreatedByUserId = user.Id }).FirstOrDefault();
            
            toDo.IsDelete = !toDo.IsDelete;
            if (toDo.IsDelete)
                toDo.IsDeleteDateTime = DateTime.Now;
            else
                toDo.IsDeleteDateTime = null;
            
            conn.Execute("UPDATE ToDoList SET IsDelete = @IsDelete, IsDeleteDateTime = @IsDeleteDateTime WHERE Id = @Id", toDo);
            
            conn.Close();

            Response.Write("degisiklik");
            return View();
        }
    }
}