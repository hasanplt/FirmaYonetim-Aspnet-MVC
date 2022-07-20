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
    public class PublicFunctions
    {

        public static User getUser(IDbConnection conn, string email)
        {
            conn.Open();
            User withEmailToUser = conn.Query<User>("SELECT * FROM [User] WHERE Email = @Email", new User() { Email = email }).FirstOrDefault();
            conn.Close();
            return withEmailToUser;
        }
        public static string dateTimeToStringEdit(DateTime dateTime)
        {
            string DateTimeString = dateTime.ToString();
            string[] DateTimeSplit = DateTimeString.Split(' ');
            string[] JustDate = DateTimeSplit[0].Split('.');
            string[] JustTime = DateTimeSplit[1].Split(':');
            string dateTimeEdit = JustDate[1] + "/" + JustDate[0] + "/" + JustDate[2] + " " + DateTimeSplit[1];
            return dateTimeEdit;
        }
        public static DateTime stringToDateTimeEdit(string date)
        {
            bool isPM = date.Contains(" PM");
            if (isPM)
                date = date.Replace(" PM", "");
            else
                date = date.Replace(" AM", "");

            var obj = date.Split(' ');
            var dateDetail = obj[0].Split('/');
            var timeDetail = obj[1].Split(':');

            int hour;
            if (isPM)
                hour = int.Parse(timeDetail[0]) + 12;
            else
                hour = int.Parse(timeDetail[0]);

            if (hour == 24)
                hour = 0;

            DateTime dateParse = new DateTime(int.Parse(dateDetail[2]), int.Parse(dateDetail[0]), int.Parse(dateDetail[1]), hour, int.Parse(timeDetail[1]), 0);
            return dateParse;
        }

    }
}