using System.Web.Http;

namespace Citas.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // CORS habilitado - agregar using System.Web.Http.Cors cuando se instale el paquete
            // var cors = new EnableCorsAttribute("*", "*", "*");
            // config.EnableCors(cors);

            // Configurar formato JSON como predeterminado
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(
                new System.Net.Http.Headers.MediaTypeHeaderValue("text/html"));

            // Rutas de Web API con atributos
            config.MapHttpAttributeRoutes();

            // Ruta predeterminada
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
