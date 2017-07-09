using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using WebApplication.Interfaces;
using WebApplication.Models.ViewModels;

namespace WebApplication.Models
{
    public class Contact : Home, IViewModel<ContactViewModel>
    {
        public ContactViewModel viewModel;

        public Contact(IService service) : base(service)
        {
            Init();
        }

        public void Init()
        {
            SetupViewModel();
        }

        public bool SendEmail(ContactViewModel viewModel)
        {
            SmtpClient googleServer = new SmtpClient("smtp.gmail.com", 587);
            googleServer.Credentials = new System.Net.NetworkCredential("victorioustournaments@gmail.com", "V1ct0rious T0urnam3nts");
            googleServer.EnableSsl = true;

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(viewModel.Email, viewModel.Name);
            mail.To.Add("victorioustournaments@gmail.com");
            mail.Subject = viewModel.Subject;
            mail.Body = viewModel.Body;

            googleServer.Send(mail);

            return true;
        }
        
        public void SetupViewModel()
        {
            viewModel = new ContactViewModel();
        }

        public void SetFields()
        {
            
        }

        public void ApplyChanges(ContactViewModel viewModel)
        {
            
        }
    }
}