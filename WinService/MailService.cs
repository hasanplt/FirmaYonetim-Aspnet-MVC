using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using Dapper;
using System.Net.Mail;
using System.Data.SqlClient;
using WinService.Models;
using System.IO;

namespace WinService
{
    public partial class MailService : ServiceBase
    {
        public Timer t;
        public MailService()
        {
            InitializeComponent();
            t = new Timer(30000);
            t.Elapsed += T_Elapsed;
        }

        public static IDbConnection conn = new SqlConnection(@"data source=HASANPC;initial catalog=FirmaYonetim;user id = sa;password = 123;");
        private void T_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                List<Activity> activities = conn.Query<Activity>("SELECT * FROM Activity").ToList();
                activities = editActivitiesAddDetails(activities);

                foreach (var activity in activities)
                {
                    TimeSpan nowAndDateRange = (DateTime)activity.Date - DateTime.Now;
                    if (!activity.IsMailSend && activity.Status == 0)
                    {
                        if (nowAndDateRange.TotalSeconds > 0)
                        {
                            // mail not send
                        }
                        else
                        {
                            // mail send
                            bool isMailSend = mailSend(activity.User.Email, activity);

                            if (isMailSend)
                                conn.Execute("UPDATE Activity SET IsMailSend = @IsMailSend WHERE Id = @Id", new Activity() { Id = activity.Id, IsMailSend = true });
                            else
                                conn.Execute("UPDATE Activity SET IsMailSend = @IsMailSend WHERE Id = @Id", new Activity() { Id = activity.Id, IsMailSend = false});

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Directory.CreateDirectory(@"D:\basarisizlog");
                FileStream fs = File.Create(@"D:\basarisizlog\deneme.txt");
                StreamWriter yaz = new StreamWriter(fs);
                yaz = new StreamWriter(fs);
                yaz.WriteLine(ex);
                yaz.Close();
                fs.Close();
                throw;
            }
           
        }

        protected override void OnStart(string[] args)
        {
            t.Start();
            
        }

        protected override void OnPause()
        {
            t.Stop();
        }
        protected override void OnContinue()
        {
            t.Start();
        }
        protected override void OnStop()
        {
            t.Stop();
        }
        protected override void OnShutdown()
        {
            t.Stop();
        }

        // functions
        private List<Activity> editActivitiesAddDetails(List<Activity> activities)
        {

            foreach (var activity in activities)
            {
                ActivityType activityType = conn.Query<ActivityType>("SELECT * FROM ActivityType WHERE Id = @Id", new ActivityType() { Id = activity.ActivityTypeId }).FirstOrDefault();
                Company company = conn.Query<Company>("SELECT * FROM Company WHERE Id = @Id", new Company() { Id = activity.CompanyId }).FirstOrDefault();
                Address address = conn.Query<Address>("SELECT * FROM Address WHERE Id = @Id", new Address() { Id = activity.AddressId }).FirstOrDefault();
                Contact contact = conn.Query<Contact>("SELECT * FROM Contact WHERE Id = @Id", new Contact() { Id = activity.ContactId }).FirstOrDefault();
                User user = conn.Query<User>("SELECT * FROM [User] WHERE Id = @Id", new User() { Id = activity.UserId }).FirstOrDefault();

                if (activityType != null)
                    activity.ActivityType = activityType;
                else
                    activity.ActivityType = new ActivityType() { };

                if (address != null)
                    activity.Address = address;
                else
                    activity.Address = new Address() { };

                if (contact != null)
                    activity.Contact = contact;
                else
                    activity.Contact = new Contact() { };

                if (company != null)
                    activity.Company = company;
                else
                    activity.Company = new Company() { };

                if (user != null)
                    activity.User = user;
                else
                    activity.User = new User() { };
            }

            return activities;
        }
        private bool mailSend(string sendMail, Activity activity)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("hasanpolat60official@gmail.com");
            mailMessage.To.Add(sendMail);
            mailMessage.Subject = "Bir Aktivitenizin Süresi Geçti!";
            mailMessage.Body = activity.Title + " isimli " + activity.ActivityType.Text + " türündeki aktivitenizin süresi geçmiştir ve durumu süresi geçti olarak değiştirilmiştir.";
            SmtpClient smtp = new SmtpClient();
            smtp.Credentials = new System.Net.NetworkCredential("hasanpolat60official@gmail.com", "lcrvmgxnftsxlfkv");
            smtp.Port = 587;
            smtp.Host = "smtp.gmail.com";
            smtp.EnableSsl = true;
            try
            {
                smtp.SendAsync(mailMessage, (object)mailMessage);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


    }
}
