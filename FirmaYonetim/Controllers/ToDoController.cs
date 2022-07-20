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

            conn.Open();
            ToDoList toDo = conn.Query<ToDoList>("SELECT * FROM ToDoList WHERE Id=@Id", new ToDoList() { Id = Id }).FirstOrDefault();
            conn.Close();

            toDo.EditDateTime = PublicFunctions.dateTimeToStringEdit(toDo.Date);

            ViewModel model = new ViewModel();
            model.toDo = toDo;
            model.user = PublicFunctions.getUser(conn, Session["user"].ToString());
            return View(model);
        }
        public ActionResult Delete(Guid? Id)
        {
            if (Session["user"] == null) return RedirectToAction("Index", "Login");
            if (Id == null) return Redirect("/");

            conn.Open();
            conn.Execute("DELETE FROM ToDoList WHERE Id = @Id", new ToDoList() { Id = Id });
            conn.Close();

            return Redirect("/");
        }

        // POST: ToDo
        [HttpPost]
        public ActionResult Edit(string text, string date, Guid Id)
        {
            if (Session["user"] == null) return RedirectToAction("Index", "Login");
            DateTime dateParse = PublicFunctions.stringToDateTimeEdit(date);

            conn.Open();
            conn.Execute("UPDATE ToDoList SET Text = @Text, Date = @Date WHERE Id=@Id", new ToDoList() { Id = Id, Text = text, Date = dateParse });
            conn.Close();
            
            return Redirect("/ToDo/Edit/" + Id);
        }
        [HttpPost]
        public ActionResult Add(string text, string date)
        {
            if (Session["user"] == null) return RedirectToAction("Index", "Login");
            DateTime dateParse = PublicFunctions.stringToDateTimeEdit(date);

            conn.Open();
            conn.Execute("INSERT INTO ToDoList (Text, Date) VALUES (@Text, @Date)", new ToDoList() { Text = text, Date = dateParse });
            conn.Close();

            return Redirect("/");
        }

        // API: ToDo
        public ActionResult EditStatus(Guid? Id)
        {
            if (Id == null) return View();
            if (Session["user"] == null) return RedirectToAction("Index", "Login");

            conn.Open();

            ToDoList toDo = conn.Query<ToDoList>("SELECT * FROM ToDoList WHERE Id = @Id", new ToDoList() { Id = Id }).FirstOrDefault();
            
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