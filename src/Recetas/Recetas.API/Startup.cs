    using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using Owin;
using Recetas.API.App_Start;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Text;
using System.Web.Http;

[assembly: OwinStartup(typeof(Recetas.API.Startup))]

namespace Recetas.API
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            try
            {
                Debug.WriteLine("========================================");
                Debug.WriteLine("[OWIN STARTUP] Iniciando configuración OWIN...");
                Debug.WriteLine("========================================");
                
                var config = new HttpConfiguration();

                // 1. CONFIGURAR JWT PRIMERO
                Debug.WriteLine("[OWIN STARTUP] PASO 1: Configurando JWT Authentication...");
                ConfigurarJwtValidation(app);
                Debug.WriteLine("[OWIN STARTUP] JWT configurado correctamente");

                // 2. CONFIGURAR WEB API
                Debug.WriteLine("[OWIN STARTUP] PASO 2: Configurando Web API Routes...");
                WebApiConfig.Register(config);
                Debug.WriteLine("[OWIN STARTUP]  Web API configurado");

                // 3. CONFIGURAR DEPENDENCY INJECTION
                Debug.WriteLine("[OWIN STARTUP] PASO 3: Configurando Dependency Injection...");
                DependencyConfig.Register(config);
                Debug.WriteLine("[OWIN STARTUP]  DI configurado");

                // 4. USAR WEB API CON OWIN
                Debug.WriteLine("[OWIN STARTUP] PASO 4: Registrando Web API en OWIN...");
                app.UseWebApi(config);
                Debug.WriteLine("[OWIN STARTUP]  Web API registrado en pipeline OWIN");

                Debug.WriteLine("========================================");
                Debug.WriteLine("[OWIN STARTUP]  CONFIGURACIÓN COMPLETADA EXITOSAMENTE");
                Debug.WriteLine("========================================");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("========================================");
                Debug.WriteLine("[OWIN STARTUP ERROR]  ERROR CRÍTICO EN CONFIGURACIÓN:");
                Debug.WriteLine(string.Format("  Mensaje: {0}", ex.Message));
                Debug.WriteLine(string.Format("  Tipo: {0}", ex.GetType().FullName));
                Debug.WriteLine(string.Format("  StackTrace: {0}", ex.StackTrace));
                
                if (ex.InnerException != null)
                {
                    Debug.WriteLine(string.Format("  InnerException: {0}", ex.InnerException.Message));
                    Debug.WriteLine(string.Format("  InnerStack: {0}", ex.InnerException.StackTrace));
                }
                Debug.WriteLine("========================================");
                
                throw;
            }
        }

        private void ConfigurarJwtValidation(IAppBuilder app)
        {
            try
            {
                var secretKey = ConfigurationManager.AppSettings["JwtSecretKey"];
                var issuer = ConfigurationManager.AppSettings["JwtIssuer"];
                var audience = ConfigurationManager.AppSettings["JwtAudience"];

                Debug.WriteLine("[JWT CONFIG] Leyendo configuración desde Web.config...");
                Debug.WriteLine(string.Format("[JWT CONFIG]   SecretKey presente: {0}", !string.IsNullOrEmpty(secretKey)));
                Debug.WriteLine(string.Format("[JWT CONFIG]   Issuer: {0}", issuer ?? "NULL"));
                Debug.WriteLine(string.Format("[JWT CONFIG]   Audience: {0}", audience ?? "NULL"));

                if (string.IsNullOrEmpty(secretKey))
                {
                    throw new InvalidOperationException(
                        "JwtSecretKey no configurada en Web.config. " +
                        "Agrega: <add key=\"JwtSecretKey\" value=\"...\" />");
                }

                if (secretKey.Length < 32)
                {
                    Debug.WriteLine(string.Format("[JWT WARNING] La clave tiene solo {0} caracteres. Se recomienda al menos 32.", secretKey.Length));
                }

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

                Debug.WriteLine("[JWT CONFIG] Configurando JwtBearerAuthentication...");
                
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
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(5)
                    }
                });

                Debug.WriteLine("[JWT CONFIG]  Middleware JWT configurado correctamente");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("[JWT CONFIG ERROR] Error al configurar JWT: {0}", ex.Message));
                throw;
            }
        }
    }
}