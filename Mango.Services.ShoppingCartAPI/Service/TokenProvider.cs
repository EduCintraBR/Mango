using Mango.Services.ShoppingCartAPI.Service.IService;
using Newtonsoft.Json.Linq;

namespace Mango.Services.ShoppingCartAPI.Service
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public TokenProvider(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public string? GetToken()
        {
            string? token = null;

            bool? hasToken = _contextAccessor.HttpContext?.Request.Cookies.TryGetValue("JWTToken", out token);

            return hasToken is true ? token : null;
        }
    }
}
