using System.Web.Http;
using Unity;
using Unity.WebApi;
using MediatR;
using Personas.Application.CommandHandlers;
using Personas.Application.Commands;
using Personas.Application.DTOs;
using Personas.Application.Interfaces;
using Personas.Application.Queries;
using Personas.Application.QueryHandlers;
using Personas.Domain.Interfaces;
using Personas.Infrastructure.Persistence;
using Personas.Infrastructure.Repositories;
using Personas.Infrastructure.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Lifetime;
using System.Diagnostics;

namespace Personas.API.App_Start
{
    /// <summary>
    /// Configuración de Unity Container con MediatR (CQRS Pattern)
    /// ✅ Reemplaza SimpleMediator por MediatR real con inyección automática
    /// </summary>
    public static class UnityConfig
    {
        // ✅ CORRECCIÓN: Recibir HttpConfiguration como parámetro
        public static void RegisterComponents(HttpConfiguration config)
        {
            try
            {
                Debug.WriteLine("=== INICIO: Configuración de Unity Container ===");
                var container = new UnityContainer();

                // ====================================
                // 1️⃣ INFRAESTRUCTURA - DbContext
                // ====================================
                Debug.WriteLine("[1/7] Registrando DbContext...");
                container.RegisterType<PersonasDbContext>(
                    new HierarchicalLifetimeManager());
                Debug.WriteLine("✅ DbContext registrado");

                // ====================================
                // 2️⃣ REPOSITORIOS
                // ====================================
                Debug.WriteLine("[2/7] Registrando Repositorios...");
                container.RegisterType<IPersonaRepository, PersonaRepository>(
                    new HierarchicalLifetimeManager());
                container.RegisterType<IUsuarioRepository, UsuarioRepository>(
                    new HierarchicalLifetimeManager());
                Debug.WriteLine("✅ Repositorios registrados");

                // ====================================
                // 3️⃣ SERVICIOS DE SEGURIDAD
                // ====================================
                Debug.WriteLine("[3/7] Registrando Servicios de Seguridad...");
                container.RegisterType<ITokenService, JwtTokenService>(
                    new ContainerControlledLifetimeManager());
                Debug.WriteLine("✅ Servicios de seguridad registrados");

                // ====================================
                // 4️⃣ MEDIATR - Configuración del Mediador
                // ====================================
                Debug.WriteLine("[4/7] Registrando MediatR...");
                container.RegisterType<IMediator, Mediator>(
                    new HierarchicalLifetimeManager());

                // ServiceFactory: Unity resuelve handlers dinámicamente
                container.RegisterInstance<ServiceFactory>(serviceType =>
                {
                    try
                    {
                        Debug.WriteLine(string.Format("ServiceFactory: Resolviendo tipo {0}", 
                            serviceType.Name));

                        var enumerableType = serviceType
                            .GetInterfaces()
                            .Concat(new[] { serviceType })
                            .FirstOrDefault(t => t.IsGenericType && 
                                t.GetGenericTypeDefinition() == typeof(IEnumerable<>));

                        if (enumerableType != null)
                        {
                            var result = container.ResolveAll(enumerableType.GetGenericArguments()[0]);
                            Debug.WriteLine(string.Format("✅ ResolveAll exitoso para {0}", 
                                serviceType.Name));
                            return result;
                        }

                        if (container.IsRegistered(serviceType))
                        {
                            var result = container.Resolve(serviceType);
                            Debug.WriteLine(string.Format("✅ Resolve exitoso para {0}", 
                                serviceType.Name));
                            return result;
                        }

                        Debug.WriteLine(string.Format("⚠️ Tipo no registrado: {0}", 
                            serviceType.Name));
                        return null;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(string.Format("❌ Error resolviendo {0}: {1}", 
                            serviceType.Name, ex.Message));
                        return null;
                    }
                });
                Debug.WriteLine("✅ MediatR y ServiceFactory registrados");

                // ====================================
                // 5️⃣ COMMAND HANDLERS (Escritura)
                // ====================================
                Debug.WriteLine("[5/7] Registrando Command Handlers...");
                
                container.RegisterType<IRequestHandler<CrearPersonaCommand, int>,
                    CrearPersonaCommandHandler>(new HierarchicalLifetimeManager());
                Debug.WriteLine("  - CrearPersonaCommandHandler registrado");

                container.RegisterType<IRequestHandler<ActualizarPersonaCommand, Unit>,
                    ActualizarPersonaCommandHandler>(new HierarchicalLifetimeManager());
                Debug.WriteLine("  - ActualizarPersonaCommandHandler registrado");

                container.RegisterType<IRequestHandler<EliminarPersonaCommand, Unit>,
                    EliminarPersonaCommandHandler>(new HierarchicalLifetimeManager());
                Debug.WriteLine("  - EliminarPersonaCommandHandler registrado");

                container.RegisterType<IRequestHandler<AutenticarUsuarioCommand, TokenResponseDto>,
                    AutenticarUsuarioCommandHandler>(new HierarchicalLifetimeManager());
                Debug.WriteLine("  - AutenticarUsuarioCommandHandler registrado");
                
                Debug.WriteLine("✅ Command Handlers registrados");

                // ====================================
                // 6️⃣ QUERY HANDLERS (Lectura)
                // ====================================
                Debug.WriteLine("[6/7] Registrando Query Handlers...");
                
                container.RegisterType<IRequestHandler<ObtenerTodasPersonasQuery, IEnumerable<PersonaDto>>,
                    ObtenerTodasPersonasQueryHandler>(new HierarchicalLifetimeManager());
                Debug.WriteLine("  - ObtenerTodasPersonasQueryHandler registrado");

                container.RegisterType<IRequestHandler<ObtenerPersonaPorIdQuery, PersonaDto>,
                    ObtenerPersonaPorIdQueryHandler>(new HierarchicalLifetimeManager());
                Debug.WriteLine("  - ObtenerPersonaPorIdQueryHandler registrado");

                container.RegisterType<IRequestHandler<ObtenerMedicosQuery, IEnumerable<PersonaDto>>,
                    ObtenerMedicosQueryHandler>(new HierarchicalLifetimeManager());
                Debug.WriteLine("  - ObtenerMedicosQueryHandler registrado");

                container.RegisterType<IRequestHandler<ObtenerPacientesQuery, IEnumerable<PersonaDto>>,
                    ObtenerPacientesQueryHandler>(new HierarchicalLifetimeManager());
                Debug.WriteLine("  - ObtenerPacientesQueryHandler registrado");
                
                Debug.WriteLine("✅ Query Handlers registrados");

                // ====================================
                // 7️⃣ REGISTRAR DEPENDENCY RESOLVER
                // ====================================
                Debug.WriteLine("[7/7] Registrando Unity DependencyResolver en Web API...");
                // ✅ CORRECCIÓN: Usar el config pasado como parámetro
                config.DependencyResolver = new UnityDependencyResolver(container);
                Debug.WriteLine("✅ DependencyResolver configurado");

                Debug.WriteLine("=== FIN: Unity Container configurado exitosamente ===");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("❌❌❌ ERROR CRÍTICO en UnityConfig: {0}", ex.Message));
                Debug.WriteLine(string.Format("StackTrace: {0}", ex.StackTrace));
                throw;
            }
        }
    }
}