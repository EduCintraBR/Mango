using MailKit.Net.Smtp;
using Mango.Services.EmailAPI.Data;
using Mango.Services.EmailAPI.Models;
using Mango.Services.EmailAPI.Models.Dto;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System.Text;

namespace Mango.Services.EmailAPI.Services
{
    public class EmailService : IEmailService
    {
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
            await LogAndEmail(message, "appscintra@gmail.com", "Uma nova conta foi cadastrada!");
        }

        public void SendEmail(string toAddress, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("CintraShop", "appscintra@gmail.com"));
            message.To.Add(new MailboxAddress("", toAddress));
            message.Subject = subject;

            message.Body = new TextPart("plain")
            {
                Text = body
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("appscintra", "$(educin258852)");
                client.Send(message);
                client.Disconnect(true);
            }
        }

        private async Task<bool> LogAndEmail(string message, string email, string? subject)
        {
            try
            {
                //SendEmail(email, subject, message);

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
