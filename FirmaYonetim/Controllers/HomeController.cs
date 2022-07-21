using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;
using FirmaYonetim.Models;

namespace FirmaYonetim.Controllers
{
    public class HomeController : Controller
    {
        IDbConnection conn = Connection.conn;

        // GET: Home
        public ActionResult Index()
        {
            if (Session["user"] == null) return RedirectToAction("Index", "Login");

            User withEmailToUser = PublicFunctions.getUser(conn, Session["user"].ToString());

            conn.Open();

            // Todo List and Edit Details
            List<ToDoList> toDoList = conn.Query<ToDoList>("SELECT * FROM ToDoList WHERE CreatedByUserId = @CreatedByUserId", new ToDoList() { CreatedByUserId = withEmailToUser.Id }).ToList();
            toDoList = editToDoList(toDoList);

            // Wrong Logins Add To User and Delete
            List<UserWrongLoginLog> userWrongLoginLogs = conn.Query<UserWrongLoginLog>("SELECT * FROM [UserWrongLoginLog] WHERE UserId = @UserId", new UserWrongLoginLog() { UserId = (Guid)withEmailToUser.Id }).ToList();
            foreach (var item in userWrongLoginLogs)
            {
                conn.Execute("DELETE FROM [UserWrongLoginLog] WHERE Id=@Id", item);
            }
            withEmailToUser.UserWrongLoginLogs = userWrongLoginLogs;

            // Count Istatics
            int countCompany = conn.Query<int>("SELECT COUNT(*) FROM Company WHERE IsDelete = @IsDelete and CreatedByUserId = @CreatedByUserId", new Company() { IsDelete = false, CreatedByUserId = (Guid)withEmailToUser.Id }).FirstOrDefault();
            int countContact = conn.Query<int>("SELECT COUNT(*) FROM Contact WHERE IsDelete = @IsDelete and CreatedByUserId = @CreatedByUserId", new Contact() { IsDelete = false, CreatedByUserId = (Guid)withEmailToUser.Id }).FirstOrDefault();
            int countActivityWaiting = conn.Query<int>("SELECT COUNT(*) FROM Activity WHERE Status = @Status and UserId = @UserId", new Activity() { Status = 0, UserId = (Guid)withEmailToUser.Id }).FirstOrDefault();
            int countActivityFinish = conn.Query<int>("SELECT COUNT(*) FROM Activity WHERE Status != @Status and UserId = @UserId", new Activity() { Status = 0, UserId = (Guid)withEmailToUser.Id }).FirstOrDefault();

            conn.Close();

            return View(new ViewModel()
            {
                toDoList = toDoList,
                countCompany = countCompany,
                countContact = countContact,
                countActivityWaiting = countActivityWaiting,
                countActivityFinish = countActivityFinish,
                user = withEmailToUser
            });
        }

        // FUNCTİONS
        private List<ToDoList> editToDoList(List<ToDoList> toDoList)
        {
            List<ToDoList> deleteList = new List<ToDoList>();

            foreach (var todo in toDoList)
            {
                TimeSpan tsDateRange = todo.Date - DateTime.Now;
                todo.dateRangeHours = tsDateRange.Hours + tsDateRange.Days * 24;
                if (todo.IsDelete)
                {
                    TimeSpan deleteRangeNow = DateTime.Now - (DateTime)todo.IsDeleteDateTime;
                    if (deleteRangeNow.Days >= 3)
                    {
                        conn.Execute("DELETE FROM ToDoList WHERE Id = @Id", new ToDoList() { Id = todo.Id });
                        deleteList.Add(todo);
                    }
                }
            }

            foreach (var deleteItem in deleteList)
            {
                toDoList.Remove(deleteItem);
            }

            return toDoList;
        }

    }
}