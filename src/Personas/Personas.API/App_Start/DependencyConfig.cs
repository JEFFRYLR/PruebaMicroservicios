using System.Web.Http;

namespace Personas.API.App_Start
{
    /// <summary>
    /// Configuración de Inyección de Dependencias - Delegado a Unity
    /// ✅ Unity maneja toda la resolución de dependencias automáticamente
    /// ⚠️ Este archivo se mantiene por compatibilidad pero ya NO se usa
    /// ⚠️ La configuración real está en UnityConfig.cs
    /// </summary>
    public static class DependencyConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Unity ya está configurado en Startup.cs -> UnityConfig.RegisterComponents()
            // SimpleMediator ha sido eliminado y reemplazado por MediatR real
        }
    }
}
