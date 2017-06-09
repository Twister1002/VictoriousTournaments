using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using SendGrid;
using SendGrid.Helpers.Mail;


namespace DatabaseLib
{
    public class EmailService
    {
        IUnitOfWork unitOfWork;
        string responseHeaders;

        public EmailService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        //private SendGridMessage GenerateBaseEmil()
        //{


        //    return msg;
        //}

        public bool SendAccountInviteEmail(AccountInviteModel invite)
        {
            try
            {
                var msg = new SendGridMessage();

              
                AccountModel sender = unitOfWork.AccountRepo.Get(invite.SentByID);
                msg.SetFrom(new EmailAddress(sender.Email, sender.GetFullName()));

                var recepiants = new List<EmailAddress>()
                {
                    new EmailAddress(invite.SentToEmail)
                };

                msg.AddTos(recepiants);

                string subject = "You have been invited to create an account on Victorious by " + sender.GetFullName();
                msg.SetSubject(subject);

                msg.AddContent(MimeType.Html, "<p> Visit {insert URL here}" + invite.AccountInviteCode + " to create your account </p>");

                SendGridClient client = new SendGridClient("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");

                Execute(client, msg).Wait();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }


        private static async Task Execute(SendGridClient client, SendGridMessage msg)
        {

            var response = await client.SendEmailAsync(msg);
            Console.WriteLine(response.Headers.ToString());
            
        }
    }
}
