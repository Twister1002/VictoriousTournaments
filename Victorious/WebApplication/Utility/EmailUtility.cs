using DatabaseLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using WebApplication.Models.ViewModels;

namespace WebApplication.Utility
{
    public class EmailUtility
    {
        private SmtpClient googleServer;
        private MailMessage mail;
        public Exception e;

        public EmailUtility()
        {
            googleServer = new SmtpClient("smtp.gmail.com", 587);
            googleServer.Credentials = new System.Net.NetworkCredential("victorioustournaments@gmail.com", "V1ct0rious T0urnam3nts");
            googleServer.EnableSsl = true;

            mail = new MailMessage();
            mail.From = new MailAddress("victorioustournaments@gmail.com", "Victorious Tournaments");
        }

        public bool EmailForgottenPassword(AccountModel accountModel, AccountForgetModel forgotModel)
        {
            try
            {
                // Replace the constants
                String emailContents = LoadScript("ForgotEmail.html")
                    .Replace("{ACCOUNT_USERNAME}", accountModel.Username)
                    .Replace("{ACCOUNT_TOKEN}", forgotModel.Token);

                // Enter in the email's information
                mail.To.Add(accountModel.Email);
                mail.Subject = "Forgotten Password";
                mail.Body = emailContents;

                googleServer.Send(mail);
                return true;
            }
            catch (Exception e)
            {
                this.e = e;
                return false;
            }
        }

        public bool EmailInquiry(ContactViewModel model)
        {
            try
            {
                mail.To.Add("victorioustournaments@gmail.com");
                mail.Subject = model.Subject;
                mail.Body = "The person named " + model.Name + " <" + model.Email + "> has sent a message: \n\n" + model.Body;

                googleServer.Send(mail);
                return true;
            }
            catch (Exception e)
            {
                this.e = e;
                return false;
            }
        }

        private String LoadScript(String name)
        {
            String path = System.Web.Hosting.HostingEnvironment.MapPath(@"~/Utility/Scripts/" + name);
            String emailContents = File.ReadAllText(path);

            return emailContents;
        }
    }
}