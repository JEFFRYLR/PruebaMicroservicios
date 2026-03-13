using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using Owin;
using Personas.API.App_Start;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Text;
using System.Web.Http;

[assembly: OwinStartup(typeof(Personas.API.Startup))]

namespace Personas.API
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            try
            {
                Debug.WriteLine("========================================");
                Debug.WriteLine("=== INICIO: Configuración OWIN ===");
                Debug.WriteLine("========================================");

                var config = new HttpConfiguration();

                // ====================================
                // 1️⃣ Configurar JWT Authentication
                // ====================================
                Debug.WriteLine("[1/4] Configurando JWT Authentication...");
                ConfigurarJwtAuthentication(app);
                Debug.WriteLine("✅ JWT Authentication configurado");

                // ====================================
                // 2️⃣ Configurar Web API Routes
                // ====================================
                Debug.WriteLine("[2/4] Configurando Web API Routes...");
                WebApiConfig.Register(config);
                Debug.WriteLine("✅ Web API Routes configurado");

                // ====================================
                // 3️⃣ Configurar Unity + MediatR
                // ====================================
                Debug.WriteLine("[3/4] Configurando Unity + MediatR...");
                // ✅ CORRECCIÓN: Pasar el config a Unity
                UnityConfig.RegisterComponents(config);
                Debug.WriteLine("✅ Unity + MediatR configurado");

                // ====================================
                // 4️⃣ Iniciar Web API con OWIN
                // ====================================
                Debug.WriteLine("[4/4] Iniciando Web API con OWIN...");
                app.UseWebApi(config);
                Debug.WriteLine("✅ Web API iniciado");

                Debug.WriteLine("========================================");
                Debug.WriteLine("=== FIN: OWIN configurado exitosamente ===");
                Debug.WriteLine("========================================");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("========================================");
                Debug.WriteLine("❌❌❌ ERROR CRÍTICO EN STARTUP ❌❌❌");
                Debug.WriteLine(string.Format("Mensaje: {0}", ex.Message));
                Debug.WriteLine(string.Format("StackTrace: {0}", ex.StackTrace));
                Debug.WriteLine("========================================");
                throw;
            }
        }

        private void ConfigurarJwtAuthentication(IAppBuilder app)
        {
            try
            {
                Debug.WriteLine("  → Leyendo configuración JWT desde Web.config...");

                var secretKey = ConfigurationManager.AppSettings["JwtSecretKey"];
                var issuer = ConfigurationManager.AppSettings["JwtIssuer"];
                var audience = ConfigurationManager.AppSettings["JwtAudience"];

                Debug.WriteLine(string.Format("    - Issuer: {0}", issuer ?? "(null)"));
                Debug.WriteLine(string.Format("    - Audience: {0}", audience ?? "(null)"));
                Debug.WriteLine(string.Format("    - SecretKey: {0}",
                    string.IsNullOrEmpty(secretKey) ? "(null)" : "***configurada***"));

                if (string.IsNullOrEmpty(secretKey))
                {
                    throw new InvalidOperationException(
                        "JwtSecretKey no configurada en Web.config");
                }

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

                Debug.WriteLine("  → Configurando JwtBearerAuthentication...");
                app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = AuthenticationMode.Active,
                    TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = securityKey,
                        ValidateIssuer = true,
                        ValidIssuer = issuer,
                        ValidateAudience = true,
                        ValidAudience = audience,
                        ValidateLifetime = true
                    }
                });

                Debug.WriteLine("  ✅ JWT configurado correctamente");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("  ❌ Error configurando JWT: {0}", ex.Message));
                throw;
            }
        }
    }
}