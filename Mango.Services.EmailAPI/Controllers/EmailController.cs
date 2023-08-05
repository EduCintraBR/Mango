using Mango.Services.EmailAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.EmailAPI.Controllers
{
    [Route("api/email")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            this._emailService = emailService;
        }

        [HttpPost]
        public IActionResult SendMail(string toAddress, string subject, string body)
        {
            try
            {
                _emailService.SendEmail(toAddress, subject, body);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
