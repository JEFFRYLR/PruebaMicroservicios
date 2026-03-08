using MediatR;
using Personas.API.Controllers;
using Personas.Application.CommandHandlers;
using Personas.Application.Commands;
using Personas.Application.DTOs;
using Personas.Application.Queries;
using Personas.Application.QueryHandlers;
using Personas.Domain.Interfaces;
using Personas.Infrastructure.Persistence;
using Personas.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;

namespace Personas.API.App_Start
{
    /// <summary>
    /// Configuración de Inyección de Dependencias con CQRS + MediatR
    /// </summary>
    public static class DependencyConfig
    {
        public static void Register(System.Web.Http.HttpConfiguration config)
        {
            config.DependencyResolver = new PersonasDependencyResolver();
        }
    }

    public class PersonasDependencyResolver : IDependencyResolver
    {
        public IDependencyScope BeginScope()
        {
            return new PersonasDependencyScope();
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
        }
    }

    public class PersonasDependencyScope : IDependencyScope
    {
        private PersonasDbContext _context;
        private IPersonaRepository _repository;

        public object GetService(Type serviceType)
        {
            try
            {
                if (serviceType == typeof(PersonasController))
                {
                    _context = new PersonasDbContext();
                    _repository = new PersonaRepository(_context);
                    var mediator = new SimpleMediator(_repository);
                    return new PersonasController(mediator);
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
            _context?.Dispose();
        }
    }

    /// <summary>
    /// Implementación simple de IMediator para Personas (CQRS Pattern)
    /// </summary>
    public class SimpleMediator : IMediator
    {
        private readonly IPersonaRepository _repository;

        public SimpleMediator(IPersonaRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async System.Threading.Tasks.Task<TResponse> Send<TResponse>(IRequest<TResponse> request, System.Threading.CancellationToken cancellationToken = default)
        {
            // COMMANDS
            if (request is CrearPersonaCommand crearCmd)
            {
                var handler = new CrearPersonaCommandHandler(_repository);
                var result = await handler.Handle(crearCmd, cancellationToken);
                return (TResponse)(object)result;
            }
            else if (request is ActualizarPersonaCommand actualizarCmd)
            {
                var handler = new ActualizarPersonaCommandHandler(_repository);
                var result = await handler.Handle(actualizarCmd, cancellationToken);
                return (TResponse)(object)result;
            }
            else if (request is EliminarPersonaCommand eliminarCmd)
            {
                var handler = new EliminarPersonaCommandHandler(_repository);
                var result = await handler.Handle(eliminarCmd, cancellationToken);
                return (TResponse)(object)result;
            }
            // QUERIES
            else if (request is ObtenerTodasPersonasQuery todasQuery)
            {
                var handler = new ObtenerTodasPersonasQueryHandler(_repository);
                var result = await handler.Handle(todasQuery, cancellationToken);
                return (TResponse)(object)result;
            }
            else if (request is ObtenerPersonaPorIdQuery porIdQuery)
            {
                var handler = new ObtenerPersonaPorIdQueryHandler(_repository);
                var result = await handler.Handle(porIdQuery, cancellationToken);
                return (TResponse)(object)result;
            }
            else if (request is ObtenerMedicosQuery medicosQuery)
            {
                var handler = new ObtenerMedicosQueryHandler(_repository);
                var result = await handler.Handle(medicosQuery, cancellationToken);
                return (TResponse)(object)result;
            }
            else if (request is ObtenerPacientesQuery pacientesQuery)
            {
                var handler = new ObtenerPacientesQueryHandler(_repository);
                var result = await handler.Handle(pacientesQuery, cancellationToken);
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
