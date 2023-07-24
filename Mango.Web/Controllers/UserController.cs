using Mango.Web.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Web.Controllers
{
    public class UserController : Controller
    {
        public IActionResult UserIndex()
        {
            var listUser = new List<UserDto>();
            return View(listUser);
        }
    }
}
