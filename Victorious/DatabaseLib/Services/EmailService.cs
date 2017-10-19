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
        //string responseHeaders;
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

        /// <summary>
        /// Sends an account invite via email.
        /// </summary>
        /// <param name="invite"> AccountInvite object that contains the info needed to fill out the email template. </param>
        /// <returns></returns>
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
                unitOfWork.SetException(ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Sends a tournament invite to specified recipieants.
        /// </summary>
        /// <param name="invite"> The TournamentInvite that contains the data needed to fill out the email template. </param>
        /// <param name="url"> The URL of the tournament that people are being inbited to. </param>
        /// <param name="recepiantEmails"> A List of email addresses that will receive the email. </param>
        /// <returns> Returns true if the email is sent, elese returns false. </returns>
        public bool SendTournamentInviteEmail(TournamentInviteModel invite, string url, List<string> recipientEmails)
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
                foreach (var recepiant in recipientEmails)
                {
                    recepiants.Add(new EmailAddress(recepiant));
                }
                msg.AddTos(recepiants);


                Execute(client, msg).Wait();
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Sends a verification email to ensure the email the user entered at account creation is valid.
        /// </summary>
        /// <param name="userEmail"> The user's email address. </param>
        /// <param name="verificationCode"> The code to include in the email. </param>
        /// <returns> Returns true if the email is sent, else returns false. </returns>
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
                unitOfWork.SetException(ex);
                return false;  
            }
            return true;
        }

        #region MailingList

        /// <summary>
        /// Adds an email address to the mailing list. 
        /// </summary>
        /// <param name="email"> The email address to be added. </param>
        public void AddEmailToMailingList(string email)
        {
            unitOfWork.MailingListRepo.Add(new MailingListModel() { EmailAddress = email });
        }

        /// <summary>
        /// Retreives the entire mailing list.
        /// </summary>
        /// <returns> Returns a List of strings. </returns>
        public List<string> GetMailingList()
        {
            List<string> mailingList = new List<string>();
            foreach (var email in unitOfWork.MailingListRepo.GetAll())
            {
                mailingList.Add(email.EmailAddress);
            }
            return mailingList;
        }

        /// <summary>
        /// Removes an email from the mailing list.
        /// </summary>
        /// <param name="email"> The email address to be removed. </param>
        public void RemoveEmailFromMailingList(string email)
        {
            MailingListModel emailToDelete = unitOfWork.MailingListRepo.GetSingle(x => x.EmailAddress == email);
            unitOfWork.MailingListRepo.DeleteEntity(emailToDelete);
        }

        /// <summary>
        /// Sends an email to all addresses on the mailing list.
        /// </summary>
        /// <param name="subject"> The subject of the email. </param>
        /// <param name="message"> The main body of the email. </param>
        /// <returns> Returns true if the email is sent, else returns false. </returns>
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
                unitOfWork.SetException(ex);
                return false;
            }
            return true;
        }

        #endregion

        #region PasswordReset



        #endregion

        /// <summary>
        /// Executes the sending of the email.
        /// </summary>
        /// <param name="client"> The SendGridClient that allows api access. </param>
        /// <param name="msg"> The SendGridMessage that is being sent. </param>
        /// <returns></returns>
        private static async Task Execute(SendGridClient client, SendGridMessage msg)
        {

            var response = await client.SendEmailAsync(msg);
            Console.WriteLine(response.Headers.ToString());

        }
    }
}
