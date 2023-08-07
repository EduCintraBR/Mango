using Mango.Services.RewardsAPI.Message;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Mango.Services.RewardsAPI.Services
{
    public class RewardsService : IRewardsService
    {
        private const string MailSender = "appscintra@gmail.com";
        private const string PasswordMail = "nesfmikribeusoei";
        private const string SmtpAddress = "smtp.gmail.com";
        private const int PortNumber = 587;
        private DbContextOptions<AppDbContext> _dbOptions;

        public RewardsService(DbContextOptions<AppDbContext> dbOptions)
        {
            this._dbOptions = dbOptions;
        }

        public async Task UpdateRewards(RewardsMessage rewardsMessage)
        {
            throw new NotImplementedException();
        }

        public void SendEmail(string toAddress, string subject, string body)
        {
            using(MailMessage  mailMessage = new MailMessage())
            {
                mailMessage.Sender = new MailAddress(MailSender);
                mailMessage.From = new MailAddress(MailSender);
                mailMessage.To.Add(new MailAddress(toAddress));
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient(SmtpAddress, PortNumber))
                {
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(MailSender, PasswordMail);

                    smtp.Send(mailMessage);
                }
            }
        }
    }
}
