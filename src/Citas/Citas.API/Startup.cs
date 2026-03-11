using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using Owin;
using Citas.API.App_Start;
using System.Configuration;
using System.Text;
using System.Web.Http;

[assembly: OwinStartup(typeof(Citas.API.Startup))]

namespace Citas.API
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            // Configurar JWT Validation (sin endpoint de login)
            ConfigurarJwtValidation(app);

            // Configurar Web API
            WebApiConfig.Register(config);
            DependencyConfig.Register(config);

            // Usar Web API con OWIN
            app.UseWebApi(config);
        }

        private void ConfigurarJwtValidation(IAppBuilder app)
        {
            var secretKey = ConfigurationManager.AppSettings["JwtSecretKey"];
            var issuer = ConfigurationManager.AppSettings["JwtIssuer"];
            var audience = ConfigurationManager.AppSettings["JwtAudience"];

            if (string.IsNullOrEmpty(secretKey))
            {
                throw new System.InvalidOperationException(
                    "JwtSecretKey no configurada en Web.config");
            }

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey));

            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = securityKey,
                    ValidateLifetime = true
                }
            });
        }
    }
}