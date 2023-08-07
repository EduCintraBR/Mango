using Mango.Services.EmailAPI.Data;
using Mango.Services.EmailAPI.Message;
using Mango.Services.EmailAPI.Models;
using Mango.Services.EmailAPI.Models.Dto;
using Mango.Services.EmailAPI.Utility;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Mango.Services.EmailAPI.Services
{
    public class EmailService : IEmailService
    {
        private DbContextOptions<AppDbContext> _dbOptions;
        private readonly AppConfig _appConfig;

        public EmailService(DbContextOptions<AppDbContext> dbOptions, AppConfig appConfig)
        {
            this._dbOptions = dbOptions;
            _appConfig = appConfig;
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
            await LogAndEmail(message, _appConfig.MailSender, subject);
        }

        public void SendEmail(string toAddress, string subject, string body)
        {
            using(MailMessage  mailMessage = new MailMessage())
            {
                mailMessage.Sender = new MailAddress(_appConfig.MailSender);
                mailMessage.From = new MailAddress(_appConfig.MailSender);
                mailMessage.To.Add(new MailAddress(toAddress));
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient(_appConfig.SmtpAddress, _appConfig.PortNumber))
                {
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(_appConfig.MailSender, _appConfig.PasswordMail);

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

        public async Task LogOrderPlaced(RewardsMessage rewardsMessage)
        {
            string message = $"Novo Pedido realizado. <br/> ID do Pedido: {rewardsMessage.OrderId}";
            await LogAndEmail(message, _appConfig.MailSender, null);
        }
    }
}
