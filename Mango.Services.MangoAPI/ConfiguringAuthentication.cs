using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Mango.Services.CouponAPI
{
    public class ConfiguringAuthentication
    {
        public static void Configure (IServiceCollection services, ConfigurationManager config)
        {
            var apiSection = config.GetSection("ApiSettings");

            var secret = apiSection.GetValue<string>("Secret");
            var issuer = apiSection.GetValue<string>("Issuer");
            var audience = apiSection.GetValue<string>("Audience");

            var key = Encoding.ASCII.GetBytes(secret);

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience
                };
            });
            services.AddAuthorization();
        }
    }
}
