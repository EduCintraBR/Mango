using Mango.Services.EmailAPI.Data;
using Mango.Services.EmailAPI.Models;
using Mango.Services.EmailAPI.Models.Dto;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Mango.Services.EmailAPI.Services
{
    public class EmailService : IEmailService
    {
        private const string MailSender = "appscintra@gmail.com";
        private const string PasswordMail = "$(educin258852)";
        private const string SmtpAddress = "smtp.gmail.com";
        private const int PortNumber = 587;

        private DbContextOptions<AppDbContext> _dbOptions;
        public EmailService(DbContextOptions<AppDbContext> dbOptions)
        {
            this._dbOptions = dbOptions;
        }

        public async Task EmailCartAndLog(CartDto cartDto)
        {
            StringBuilder message = new StringBuilder();

            message.AppendLine("<br>Carrinho de Compras Salvo");
            message.AppendLine("<br>Total da compra: " + String.Format("{0:c}", cartDto.CartHeader.CartTotal));
            message.Append("<br>");
            message.Append("<ul>");

            foreach (var item in cartDto.CartDetails)
            {
                message.Append("<li>");
                message.Append(item.Product.Name + " x " + item.Count);
                message.Append("</li>");
            }
            message.Append("</ul>");

            await LogAndEmail(message.ToString(), cartDto.CartHeader.Email, "Seu carrinho de compras salvo");
        }

        public async Task RegisterUserEmailAndLog(string email)
        {
            string message = $"Usuário Registrado com Sucesso <br/> Email: {email}";
            string subject = "Uma nova conta foi cadastrada!";

            SendEmail(email, subject, message);
            await LogAndEmail(message, MailSender, subject);
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

        private async Task<bool> LogAndEmail(string message, string email, string? subject)
        {
            try
            {
                EmailLogger emailLog = new() { Message = message, Email = email , EmailSent = DateTime.Now };
                
                await using var _db = new AppDbContext(_dbOptions);
                await _db.EmailLoggers.AddAsync(emailLog);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
