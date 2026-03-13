using Personas.API.App_Start;
using System;
using System.Diagnostics;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Personas.API
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Debug.WriteLine("⚠️ Global.asax.Application_Start ejecutado");
            Debug.WriteLine("⚠️ NOTA: Con OWIN, Startup.cs tiene prioridad");

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);

            // ⚠️ NOTA: Con OWIN, esto puede no ejecutarse o ser ignorado
            // La configuración real está en Startup.cs
            // UnityConfig.RegisterComponents();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            // Log cada request
            var request = HttpContext.Current.Request;
            Debug.WriteLine(string.Format("[REQUEST] {0} {1}",
                request.HttpMethod, request.Url.PathAndQuery));
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
            Debug.WriteLine("========================================");
            Debug.WriteLine("❌ Application_Error capturado:");
            Debug.WriteLine(string.Format("Mensaje: {0}", exception?.Message));
            Debug.WriteLine(string.Format("StackTrace: {0}", exception?.StackTrace));
            Debug.WriteLine("========================================");
        }
    }
}
