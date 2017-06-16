using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.IO;


namespace DatabaseLib
{
    public class EmailService
    {
        IUnitOfWork unitOfWork;
        string responseHeaders;
        SendGridClient client;

        public EmailService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;

            client = new SendGridClient("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");

        }

        //private SendGridMessage GenerateBaseEmil()
        //{


        //    return msg;
        //}

        private async Task<Response> GetTemplate(string id)
        {
            return await client.RequestAsync(SendGridClient.Method.GET, "templates" + id);

        }

        public bool SendAccountInviteEmail(AccountInviteModel invite)
        {
            try
            {
                var msg = new SendGridMessage();
                var templateId = "70cd6b26-44e7-4b62-b76b-db2c564e24ba";

                msg.TemplateId = templateId;

                AccountModel sender = unitOfWork.AccountRepo.Get(invite.SentByID);
                msg.SetFrom(new EmailAddress(sender.Email, sender.GetFullName()));

                Dictionary<string, string> substitutions = new Dictionary<string, string>()
                {
                    { "{sender_name}", sender.GetFullName().ToString()},
                    { "{account_creation_url}", "http://localhost:20346/Account/Register" }
                };

                msg.AddSubstitutions(substitutions);

                var recepiants = new List<EmailAddress>()
                {
                    new EmailAddress(invite.SentToEmail)
                };

                msg.AddTos(recepiants);

                msg.SetClickTracking(true, false);

                //string subject = "You have been invited to create an account on Victorious by " + sender.GetFullName();
                //msg.SetSubject(subject);

                //msg.AddContent(MimeType.Html, "<p> Visit {insert URL here}" + invite.AccountInviteCode + " to create your account </p>");


                Execute(client, msg).Wait();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public bool SendTournamentInviteEmail(TournamentInviteModel invite, string url, List<string> recepiantEmails)
        {
            try
            {
                var msg = new SendGridMessage();

                AccountModel sender = unitOfWork.AccountRepo.Get(invite.Tournament.CreatedByID);
                msg.SetFrom(new EmailAddress(sender.Email, sender.GetFullName()));
                var templateId = "d921bc44-497d-41b4-a670-56f4af8f37a5";
                msg.TemplateId = templateId;

               
                Dictionary<string, string> substitutions = new Dictionary<string, string>()
                {
                    { "{sender_name}", sender.GetFullName().ToString()},
                    { "{account_creation_url}", "http://localhost:20346/Account/Register" },
                    { "{tournament_title}", invite.Tournament.Title },
                    { "{game_type}", invite.Tournament.GameType.Title },
                    { "{platform}", invite.Tournament.Platform.PlatformName },
                    { "{registration_start_date}", invite.Tournament.RegistrationStartDate.ToShortDateString() },
                    { "{registration_end_date}", invite.Tournament.RegistrationEndDate.ToShortDateString() },
                    { "{tournament_start_date}", invite.Tournament.TournamentStartDate.ToShortDateString() },
                    { "{tournament_url}", url }

                };
                msg.AddSubstitutions(substitutions);

                var recepiants = new List<EmailAddress>();
                foreach (var recepiant in recepiantEmails)
                {
                    recepiants.Add(new EmailAddress(recepiant));
                }
                msg.AddTos(recepiants);


                Execute(client, msg).Wait();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public bool SendVerificationEmail(string userEmail, string verificationCode)
        {
            try
            {
                var msg = new SendGridMessage();

                msg.SetFrom(new EmailAddress("victoriouswebsite@gmail.com", "Administrator"));
                var templateId = "a942917c-bb5d-4d55-889d-bf6fff84446f";
                msg.TemplateId = templateId;

                Dictionary<string, string> substitutions = new Dictionary<string, string>()
                {
                    { "{verification_url}", "http://localhost:20346/Account/Register" },
                    { "{verification_code}", verificationCode },
                };

                msg.AddSubstitutions(substitutions);
                msg.AddTo(userEmail);

                Execute(client, msg).Wait();
            }
            catch (Exception ex)
            {
                return false;  
            }
            return true;
        }

        #region MailingList

        public void AddEmailToMailingList(string email)
        {
            unitOfWork.MailingListRepo.Add(new MailingList() { EmailAddress = email });
        }

        public List<string> GetMailingList()
        {
            List<string> mailingList = new List<string>();
            foreach (var email in unitOfWork.MailingListRepo.GetAll())
            {
                mailingList.Add(email.EmailAddress);
            }
            return mailingList;
        }

        public void RemoveEmailFromMailingList(string email)
        {
            MailingList emailToDelete = unitOfWork.MailingListRepo.GetSingle(x => x.EmailAddress == email);
            unitOfWork.MailingListRepo.DeleteEntity(emailToDelete);
        }

        public bool SendEmailToMailingList(string subject, string message)
        {
            try
            {
                var msg = new SendGridMessage();

                msg.SetFrom(new EmailAddress("victoriouswebsite@gmail.com", "Administrator"));
                var templateId = "1fde50ac-07f2-4f82-9f38-7194b77490a9";
                msg.TemplateId = templateId;

                Dictionary<string, string> substitutions = new Dictionary<string, string>()
                {
                    { "{subject}", subject },
                    { "{message}", message },
                };
                msg.AddSubstitutions(substitutions);

                List<EmailAddress> recpients = new List<EmailAddress>();
                foreach (var recipient in unitOfWork.MailingListRepo.GetAll())
                {
                    recpients.Add(new EmailAddress(recipient.EmailAddress));
                } 
                msg.AddTos(recpients);

                Execute(client, msg).Wait();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        #endregion

        private void Send(SendGridMessage email)
        {

        }

        private static async Task Execute(SendGridClient client, SendGridMessage msg)
        {

            var response = await client.SendEmailAsync(msg);
            Console.WriteLine(response.Headers.ToString());

        }
    }
}
