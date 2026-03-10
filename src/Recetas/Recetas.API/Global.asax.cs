using Recetas.API.App_Start;
using Recetas.Infrastructure.Messaging;
using System;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Recetas.API
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private static RabbitMQConsumer _rabbitMQConsumer;

        protected void Application_Start()
        {
            try
            {
                Debug.WriteLine("========================================");
                Debug.WriteLine("[RECETAS.API] Iniciando aplicación...");
                Debug.WriteLine("========================================");

                AreaRegistration.RegisterAllAreas();
                Debug.WriteLine("[RECETAS.API] Areas registradas");

                GlobalConfiguration.Configure(WebApiConfig.Register);
                Debug.WriteLine("[RECETAS.API] WebApiConfig registrado");

                // Registrar la inyección de dependencias para MediatR
                DependencyConfig.Register(GlobalConfiguration.Configuration);
                Debug.WriteLine("[RECETAS.API] DependencyConfig registrado");

                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                Debug.WriteLine("[RECETAS.API] Filtros registrados");

                RouteConfig.RegisterRoutes(RouteTable.Routes);
                Debug.WriteLine("[RECETAS.API] Rutas registradas");

                BundleConfig.RegisterBundles(BundleTable.Bundles);
                Debug.WriteLine("[RECETAS.API] Bundles registrados");

                // INICIAR CONSUMER DE RABBITMQ
                try
                {
                    Debug.WriteLine("[RECETAS.API] Intentando iniciar RabbitMQ Consumer...");
                    IniciarRabbitMQConsumer();
                    Debug.WriteLine("[RECETAS.API] RabbitMQ Consumer iniciado correctamente");
                }
                catch (Exception rabbitEx)
                {
                    Debug.WriteLine("[RECETAS.API ERROR] No se pudo iniciar RabbitMQ Consumer:");
                    Debug.WriteLine($"  Mensaje: {rabbitEx.Message}");
                    Debug.WriteLine($"  StackTrace: {rabbitEx.StackTrace}");
                    
                    // NO LANZAR EXCEPCIÓN - Permitir que la API inicie sin RabbitMQ
                    Console.WriteLine($"⚠️ API iniciará sin RabbitMQ Consumer: {rabbitEx.Message}");
                }

                Debug.WriteLine("========================================");
                Debug.WriteLine("=== Recetas.API iniciado correctamente ===");
                Debug.WriteLine("========================================");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("========================================");
                Debug.WriteLine("[ERROR CRÍTICO RECETAS.API] Error al iniciar:");
                Debug.WriteLine($"  Mensaje: {ex.Message}");
                Debug.WriteLine($"  Tipo: {ex.GetType().FullName}");
                Debug.WriteLine($"  StackTrace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"  InnerException: {ex.InnerException.Message}");
                    Debug.WriteLine($"  InnerException StackTrace: {ex.InnerException.StackTrace}");
                }
                Debug.WriteLine("========================================");
                
                throw; // Re-lanzar para que aparezca en el navegador
            }
        }

        private void IniciarRabbitMQConsumer()
        {
            try
            {
                Debug.WriteLine("[INICIO] Intentando iniciar RabbitMQ Consumer...");
                
                // ✅ Crear consumer sin dependencias de repositorio
                // El consumer creará su propio scope de BD por cada mensaje
                _rabbitMQConsumer = new RabbitMQConsumer(
                    hostName: "168.231.74.78", 
                    userName: "admin", 
                    password: "admin123"
                );

                Debug.WriteLine("[PASO 1] RabbitMQConsumer instanciado correctamente");
                
                _rabbitMQConsumer.IniciarEscucha();

                Debug.WriteLine("[PASO 2] Consumer iniciado y escuchando mensajes");
                Console.WriteLine("✅ RabbitMQ Consumer iniciado correctamente");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR COMPLETO] No se pudo iniciar RabbitMQ Consumer");
                Debug.WriteLine($"Mensaje: {ex.Message}");
                Debug.WriteLine($"Tipo: {ex.GetType().FullName}");
                Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"InnerException: {ex.InnerException.Message}");
                    Debug.WriteLine($"InnerException StackTrace: {ex.InnerException.StackTrace}");
                }
                
                Console.WriteLine($"⚠️ No se pudo iniciar RabbitMQ Consumer: {ex.Message}");
                // No lanzar excepción para que la API inicie igual
            }
        }

        protected void Application_End()
        {
            try
            {
                Debug.WriteLine("[SHUTDOWN] Liberando recursos de RabbitMQ Consumer...");
                _rabbitMQConsumer?.Dispose();
                Debug.WriteLine("[SHUTDOWN] RabbitMQ Consumer liberado correctamente");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR SHUTDOWN] Error al liberar RabbitMQ Consumer: {ex.Message}");
            }
        }
    }
}
