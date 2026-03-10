using Citas.API.Controllers;
using Citas.Application.CommandHandlers;
using Citas.Application.Commands;
using Citas.Application.DTOs;
using Citas.Application.Interfaces;
using Citas.Application.Queries;
using Citas.Application.QueryHandlers;
using Citas.Domain.Interfaces;
using Citas.Infrastructure.Messaging;
using Citas.Infrastructure.Persistence;
using Citas.Infrastructure.Repositories;
using Citas.Infrastructure.ExternalServices; // ✅ NUEVO
using MediatR;
using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;

namespace Citas.API.App_Start
{
    /// <summary>
    /// Configuración de Inyección de Dependencias
    /// </summary>
    public static class DependencyConfig
    {
        // ✅ SINGLETON: Una sola instancia de RabbitMQ Publisher para toda la aplicación
        private static IRabbitMQPublisher _rabbitMQPublisherSingleton;
        
        // ✅ NUEVO: SINGLETON del servicio externo de Personas
        private static IPersonasExternoService _personasServiceSingleton;
        private static readonly object _lock = new object();

        public static void Register(System.Web.Http.HttpConfiguration config)
        {
            // ✅ Inicializar servicios singleton al inicio
            InicializarRabbitMQPublisher();
            InicializarPersonasService();
            
            config.DependencyResolver = new CitasDependencyResolver();
        }

        private static void InicializarRabbitMQPublisher()
        {
            if (_rabbitMQPublisherSingleton == null)
            {
                lock (_lock)
                {
                    if (_rabbitMQPublisherSingleton == null)
                    {
                        try
                        {
                            System.Diagnostics.Debug.WriteLine("=== INICIALIZANDO RABBITMQ PUBLISHER SINGLETON ===");
                            _rabbitMQPublisherSingleton = new RabbitMQPublisher(
                                hostName: "168.231.74.78",
                                userName: "admin",
                                password: "admin123"
                            );
                            System.Diagnostics.Debug.WriteLine("✅ RABBITMQ PUBLISHER SINGLETON CREADO EXITOSAMENTE");
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"❌ ERROR CREANDO RABBITMQ PUBLISHER: {ex.Message}");
                            System.Diagnostics.Debug.WriteLine("⚠️ USANDO MOCK RABBITMQ PUBLISHER");
                            _rabbitMQPublisherSingleton = new MockRabbitMQPublisher();
                        }
                    }
                }
            }
        }

        // ✅ NUEVO: Inicializar servicio externo de Personas
        private static void InicializarPersonasService()
        {
            if (_personasServiceSingleton == null)
            {
                lock (_lock)
                {
                    if (_personasServiceSingleton == null)
                    {
                        try
                        {
                            System.Diagnostics.Debug.WriteLine("=== INICIALIZANDO PERSONAS SERVICE SINGLETON ===");
                            _personasServiceSingleton = new PersonasExternoService();
                            System.Diagnostics.Debug.WriteLine("✅ PERSONAS SERVICE SINGLETON CREADO EXITOSAMENTE");
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"❌ ERROR CREANDO PERSONAS SERVICE: {ex.Message}");
                            throw; // No tiene sentido continuar sin este servicio
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Obtener instancia singleton del publisher
        /// </summary>
        public static IRabbitMQPublisher GetRabbitMQPublisher()
        {
            return _rabbitMQPublisherSingleton;
        }

        /// <summary>
        /// ✅ NUEVO: Obtener instancia singleton del servicio de Personas
        /// </summary>
        public static IPersonasExternoService GetPersonasService()
        {
            return _personasServiceSingleton;
        }
    }

    public class CitasDependencyResolver : IDependencyResolver
    {
        public IDependencyScope BeginScope()
        {
            return new CitasDependencyScope();
        }

        public object GetService(Type serviceType)
        {
            return null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return new List<object>();
        }

        public void Dispose()
        {
            // ⚠️ NO disponer de los singletons aquí
        }
    }

    public class CitasDependencyScope : IDependencyScope
    {
        private CitasDbContext _context;
        private ICitaRepository _repository;

        public object GetService(Type serviceType)
        {
            try
            {
                if (serviceType == typeof(CitasController))
                {
                    _context = new CitasDbContext();
                    _repository = new CitaRepository(_context);
                    
                    // ✅ USAR LOS SINGLETONS
                    var publisher = DependencyConfig.GetRabbitMQPublisher();
                    var personasService = DependencyConfig.GetPersonasService(); // ✅ NUEVO
                    
                    var mediator = new SimpleMediator(_repository, publisher, personasService); // ✅ MODIFICADO
                    return new CitasController(mediator);
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en GetService: {ex.Message}");
                throw;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return new List<object>();
        }

        public void Dispose()
        {
            // ✅ Solo disponer del contexto
            _context?.Dispose();
        }
    }

    /// <summary>
    /// Mock de RabbitMQ para desarrollo (cuando RabbitMQ no está disponible)
    /// </summary>
    public class MockRabbitMQPublisher : IRabbitMQPublisher
    {
        public void PublicarMensaje(string queueName, object message)
        {
            System.Diagnostics.Debug.WriteLine($"[MOCK RabbitMQ] Cola: {queueName}, Mensaje: {Newtonsoft.Json.JsonConvert.SerializeObject(message)}");
        }

        public void Dispose()
        {
            // No hacer nada
        }
    }

    /// <summary>
    /// Implementación simple de IMediator que maneja todos los Commands y Queries
    /// </summary>
    public class SimpleMediator : IMediator
    {
        private readonly ICitaRepository _repository;
        private readonly IRabbitMQPublisher _publisher;
        private readonly IPersonasExternoService _personasService; // ✅ NUEVO

        public SimpleMediator(
            ICitaRepository repository, 
            IRabbitMQPublisher publisher,
            IPersonasExternoService personasService) // ✅ NUEVO PARÁMETRO
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
            _personasService = personasService ?? throw new ArgumentNullException(nameof(personasService)); // ✅ NUEVO
        }

        public async System.Threading.Tasks.Task<TResponse> Send<TResponse>(IRequest<TResponse> request, System.Threading.CancellationToken cancellationToken = default)
        {
            // COMMANDS
            if (request is AgendarCitaCommand agendarCmd)
            {
                // ✅ MODIFICADO: Pasar el servicio de Personas al handler
                var handler = new AgendarCitaCommandHandler(_repository, _personasService);
                var result = await handler.Handle(agendarCmd, cancellationToken);
                return (TResponse)(object)result;
            }
            else if (request is ActualizarEstadoCitaCommand actualizarCmd)
            {
                var handler = new ActualizarEstadoCitaCommandHandler(_repository);
                var result = await handler.Handle(actualizarCmd, cancellationToken);
                return (TResponse)(object)result;
            }
            else if (request is FinalizarCitaCommand finalizarCmd)
            {
                var handler = new FinalizarCitaCommandHandler(_repository, _publisher);
                var result = await handler.Handle(finalizarCmd, cancellationToken);
                return (TResponse)(object)result;
            }
            // QUERIES
            else if (request is ObtenerCitaPorIdQuery obtenerPorIdQuery)
            {
                var handler = new ObtenerCitaPorIdQueryHandler(_repository);
                var result = await handler.Handle(obtenerPorIdQuery, cancellationToken);
                return (TResponse)(object)result;
            }
            else if (request is ObtenerCitasPendientesQuery pendientesQuery)
            {
                var handler = new ObtenerCitasPendientesQueryHandler(_repository);
                var result = await handler.Handle(pendientesQuery, cancellationToken);
                return (TResponse)(object)result;
            }
            else if (request is ObtenerCitasPorMedicoQuery porMedicoQuery)
            {
                var handler = new ObtenerCitasPorMedicoQueryHandler(_repository);
                var result = await handler.Handle(porMedicoQuery, cancellationToken);
                return (TResponse)(object)result;
            }
            else if (request is ObtenerCitasPorPacienteQuery porPacienteQuery)
            {
                var handler = new ObtenerCitasPorPacienteQueryHandler(_repository);
                var result = await handler.Handle(porPacienteQuery, cancellationToken);
                return (TResponse)(object)result;
            }

            throw new NotSupportedException($"No se encontró handler para: {request.GetType().Name}");
        }

        public System.Threading.Tasks.Task<object> Send(object request, System.Threading.CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task Publish(object notification, System.Threading.CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task Publish<TNotification>(TNotification notification, System.Threading.CancellationToken cancellationToken = default) where TNotification : INotification
        {
            throw new NotImplementedException();
        }
    }
}
