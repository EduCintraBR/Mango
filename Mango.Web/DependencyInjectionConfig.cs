using Mango.Web.Service.IService;
using Mango.Web.Service;

namespace Mango.Web
{
    public class DependencyInjectionConfig
    {
        public static void ConfigureAll(IServiceCollection services)
        {
            services.AddScoped<IBaseService, BaseService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICouponService, CouponService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenProvider, TokenProvider>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICartService, CartService>();
        }
    }
}
