using Mango.MessageBus;
using Mango.Services.OrderAPI.Service;
using Mango.Services.OrderAPI.Service.IService;
using Mango.Services.OrderAPI.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Mango.Services.OrderAPI.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder AddAppAuthentication(this WebApplicationBuilder builder)
        {
            var apiSection = builder.Configuration.GetSection("ApiSettings");

            var secret = apiSection.GetValue<string>("Secret");
            var issuer = apiSection.GetValue<string>("Issuer");
            var audience = apiSection.GetValue<string>("Audience");

            var key = Encoding.ASCII.GetBytes(secret);

            builder.Services.AddAuthentication(opt =>
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

            return builder;
        }

        public static WebApplicationBuilder InjectDependencies(this WebApplicationBuilder builder)
        {
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<BackendApiAuthenticationHttpClientHandler>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IAzureMessageBus, AzureMessageBus>();

            return builder;
        }
    }
}
