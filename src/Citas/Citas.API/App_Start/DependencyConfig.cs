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
        private static readonly object _lock = new object();

        public static void Register(System.Web.Http.HttpConfiguration config)
        {
            // ✅ Inicializar RabbitMQ Publisher al inicio
            InicializarRabbitMQPublisher();
            
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

        /// <summary>
        /// Obtener instancia singleton del publisher
        /// </summary>
        public static IRabbitMQPublisher GetRabbitMQPublisher()
        {
            return _rabbitMQPublisherSingleton;
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
            // ⚠️ NO disponer del singleton aquí
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
                    
                    // ✅ USAR EL SINGLETON en lugar de crear nueva instancia
                    var publisher = DependencyConfig.GetRabbitMQPublisher();
                    
                    var mediator = new SimpleMediator(_repository, publisher);
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
            // ✅ Solo disponer del contexto, NO del publisher
            _context?.Dispose();
            // ⚠️ NO llamar a _publisher?.Dispose() aquí
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

        public SimpleMediator(ICitaRepository repository, IRabbitMQPublisher publisher)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        }

        public async System.Threading.Tasks.Task<TResponse> Send<TResponse>(IRequest<TResponse> request, System.Threading.CancellationToken cancellationToken = default)
        {
            // COMMANDS
            if (request is AgendarCitaCommand agendarCmd)
            {
                var handler = new AgendarCitaCommandHandler(_repository);
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
