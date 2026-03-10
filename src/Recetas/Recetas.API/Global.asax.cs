using Recetas.API.App_Start;
using Recetas.Infrastructure.Messaging;
using Recetas.Infrastructure.Persistence;
using Recetas.Infrastructure.Repositories;
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
                AreaRegistration.RegisterAllAreas();
                GlobalConfiguration.Configure(WebApiConfig.Register);

                // Registrar la inyección de dependencias para MediatR
                DependencyConfig.Register(GlobalConfiguration.Configuration);

                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                RouteConfig.RegisterRoutes(RouteTable.Routes);
                BundleConfig.RegisterBundles(BundleTable.Bundles);

                // INICIAR CONSUMER DE RABBITMQ
                IniciarRabbitMQConsumer();

                Debug.WriteLine("=== Recetas.API iniciado correctamente ===");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Error al iniciar Recetas.API: {0}", ex.Message));
                Debug.WriteLine(string.Format("StackTrace: {0}", ex.StackTrace));
                throw;
            }
        }

        private void IniciarRabbitMQConsumer()
        {
            try
            {
                Debug.WriteLine("[INICIO] Intentando iniciar RabbitMQ Consumer...");
                
                // Crear contexto y repositorio
                var context = new RecetasDbContext();
                var repository = new RecetaRepository(context);

                Debug.WriteLine("[PASO 1] Contexto y repositorio creados");

                // Crear y iniciar consumer
                _rabbitMQConsumer = new RabbitMQConsumer(repository, hostName: "168.231.74.78", userName: "admin", password: "admin123");
                
                Debug.WriteLine("[PASO 2] RabbitMQConsumer instanciado");
                
                _rabbitMQConsumer.IniciarEscucha();

                Debug.WriteLine("[PASO 3] Consumer iniciado correctamente");
                Console.WriteLine("RabbitMQ Consumer iniciado correctamente");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("[ERROR COMPLETO] No se pudo iniciar RabbitMQ Consumer"));
                Debug.WriteLine(string.Format("Mensaje: {0}", ex.Message));
                Debug.WriteLine(string.Format("Tipo: {0}", ex.GetType().FullName));
                Debug.WriteLine(string.Format("StackTrace: {0}", ex.StackTrace));
                
                if (ex.InnerException != null)
                {
                    Debug.WriteLine(string.Format("InnerException: {0}", ex.InnerException.Message));
                    Debug.WriteLine(string.Format("InnerException StackTrace: {0}", ex.InnerException.StackTrace));
                }
                
                Console.WriteLine(string.Format("No se pudo iniciar RabbitMQ Consumer: {0}", ex.Message));
                // No lanzar excepción para que la API inicie igual
            }
        }

        protected void Application_End()
        {
            // Liberar recursos al cerrar aplicación
            _rabbitMQConsumer?.Dispose();
        }
    }
}
