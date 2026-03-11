using Recetas.Infrastructure.Messaging;
using System;
using System.Diagnostics;
using System.Web;
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

                // NOTA: La configuración de Web API y DependencyConfig 
                // ahora se maneja en Startup.cs (OWIN)
                // GlobalConfiguration.Configure(WebApiConfig.Register);
                // DependencyConfig.Register(GlobalConfiguration.Configuration);

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
                    Debug.WriteLine(string.Format("  Mensaje: {0}", rabbitEx.Message));
                    Debug.WriteLine(string.Format("  StackTrace: {0}", rabbitEx.StackTrace));
                    
                    // NO LANZAR EXCEPCIÓN - Permitir que la API inicie sin RabbitMQ
                    Console.WriteLine(string.Format("⚠️ API iniciará sin RabbitMQ Consumer: {0}", rabbitEx.Message));
                }

                Debug.WriteLine("========================================");
                Debug.WriteLine("=== Recetas.API iniciado correctamente ===");
                Debug.WriteLine("========================================");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("========================================");
                Debug.WriteLine("[ERROR CRÍTICO RECETAS.API] Error al iniciar:");
                Debug.WriteLine(string.Format("  Mensaje: {0}", ex.Message));
                Debug.WriteLine(string.Format("  Tipo: {0}", ex.GetType().FullName));
                Debug.WriteLine(string.Format("  StackTrace: {0}", ex.StackTrace));
                
                if (ex.InnerException != null)
                {
                    Debug.WriteLine(string.Format("  InnerException: {0}", ex.InnerException.Message));
                    Debug.WriteLine(string.Format("  InnerException StackTrace: {0}", ex.InnerException.StackTrace));
                }
                Debug.WriteLine("========================================");
                
                throw; // Re-lanzar para que aparezca en el navegador
            }
        }

        private void IniciarRabbitMQConsumer()
        {
            Debug.WriteLine("[INICIO] Intentando iniciar RabbitMQ Consumer...");

            try
            {
                _rabbitMQConsumer = new RabbitMQConsumer();
                Debug.WriteLine("[PASO 1] RabbitMQConsumer instanciado correctamente");

                _rabbitMQConsumer.IniciarEscucha();
                Debug.WriteLine("[PASO 2] Consumer iniciado y escuchando mensajes");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("[ERROR RABBITMQ] Error al iniciar consumer: {0}", ex.Message));
                throw;
            }
        }

        protected void Application_End()
        {
            try
            {
                Debug.WriteLine("[RECETAS.API] Cerrando aplicación...");
                
                if (_rabbitMQConsumer != null)
                {
                    Debug.WriteLine("[RECETAS.API] Deteniendo RabbitMQ Consumer...");
                    _rabbitMQConsumer.Dispose();
                    Debug.WriteLine("[RECETAS.API] RabbitMQ Consumer detenido");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("[ERROR] Error al cerrar aplicación: {0}", ex.Message));
            }
        }
    }
}
