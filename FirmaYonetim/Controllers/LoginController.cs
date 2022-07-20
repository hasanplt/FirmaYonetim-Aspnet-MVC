using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FirmaYonetim.Controllers;
using FirmaYonetim.Models;
using System.Data;
using System.Data.SqlClient;

namespace FirmaYonetim.Controllers
{
    public class LoginController : Controller
    {
        IDbConnection conn = Connection.conn;

        // GET: Login
        public ActionResult Index(string status = null)
        {
            if (Session["user"] == null)
            {
                ViewBag.status = status;
                return View(); 
            }
            else
            {
                return RedirectToAction("/");
            }
        }
        public ActionResult Logout()
        {
            Session["user"] = null;
            return RedirectToAction("Index", "Login", new {@status = "logout"});
        }

        // POST: Login
        [HttpPost]
        public ActionResult PostLoginUse(string email, string password)
        {
            User UserInfos = new User() { Email = email, Password = password };
            if (IsThereEmail(UserInfos))
            {
                if (InfosIsCorrect(UserInfos))
                {
                    return LockControlAndRedirect(UserInfos);
                }
                else
                {
                    AddDbWrongLogin(UserInfos);
                    return RedirectToAction("Index", "Login", new { @status = "no" });
                }
            }
            else
            {
                return RedirectToAction("Index", "Login", new { @status = "no" });
            }
        }

        // FUNCTİONS
        private bool IsThereEmail(User UserInfos)
        {
            conn.Open();
            int isUserCount = conn.Query<int>("SELECT Count(*) FROM [User] WHERE Email = @Email", UserInfos).SingleOrDefault();
            conn.Close();

            if(isUserCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool InfosIsCorrect(User userInfos)
        {
            conn.Open();
            int isUserCount = conn.Query<int>("SELECT Count(*) FROM [User] WHERE Email = @Email and Password = @Password", userInfos).SingleOrDefault();
            conn.Close();

            if (isUserCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void AddDbWrongLogin(User UserInfos)
        {
            conn.Open();

            User QUser = conn.Query<User>("SELECT * FROM [User] WHERE Email = @Email", UserInfos).SingleOrDefault();

            QUser.LockCount++;
            UserInfos.Id = QUser.Id;
            UserInfos.LockCount = QUser.LockCount;

            UserWrongLoginLog log = new UserWrongLoginLog() { UserId = (Guid)UserInfos.Id, WrongPassword = UserInfos.Password };
            conn.Execute("INSERT INTO [UserWrongLoginLog] (UserId, WrongPassword) VALUES (@UserId, @WrongPassword)", log);

            conn.Execute("UPDATE [User] SET LockCount = @LockCount WHERE Id=@Id", UserInfos);

            if (UserInfos.LockCount >= 3)
            {
                UserInfos.LockDateTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                conn.Execute("UPDATE [User] SET LockDateTime = @LockDateTime WHERE Id=@Id", UserInfos);
            };

            conn.Close();
        }
        private ActionResult LockControlAndRedirect(User UserInfos)
        {
            conn.Open();

            User QUser = conn.Query<User>("SELECT * FROM [User] WHERE Email = @Email", UserInfos).SingleOrDefault();

            if (QUser.LockDateTime != null)
            {
                TimeSpan timeSpan = DateTime.Now - DateTime.Parse(QUser.LockDateTime.ToString());
                int Difference = (int)Math.Floor(timeSpan.TotalMinutes);
                if(Difference >= 30)
                {

                    QUser.LockDateTime = null;
                    QUser.LockCount = 0;
                    conn.Execute("UPDATE [User] SET LockDateTime = @LockDateTime, LockCount = @LockCount WHERE Id=@Id", QUser);

                    conn.Close();

                    Session["user"] = UserInfos.Email;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    conn.Close();
                    return RedirectToAction("Index", "Login", new { @status = "yourAccountIsLock" });
                }
            }
            else
            {

                QUser.LockDateTime = null;
                QUser.LockCount = 0;
                conn.Execute("UPDATE [User] SET LockDateTime = @LockDateTime, LockCount = @LockCount WHERE Id=@Id", QUser);

                conn.Close();
                Session["user"] = UserInfos.Email;
                return RedirectToAction("Index", "Home");
            }

        }

    }
}