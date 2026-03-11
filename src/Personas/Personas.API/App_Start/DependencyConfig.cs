using MediatR;
using Personas.API.Controllers;
using Personas.Application.CommandHandlers;
using Personas.Application.Commands;
using Personas.Application.DTOs;
using Personas.Application.Interfaces;  // NUEVO
using Personas.Application.Queries;
using Personas.Application.QueryHandlers;
using Personas.Domain.Interfaces;
using Personas.Infrastructure.Persistence;
using Personas.Infrastructure.Repositories;
using Personas.Infrastructure.Security;  // NUEVO
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
        private IPersonaRepository _personaRepository;
        private IUsuarioRepository _usuarioRepository;  // NUEVO

        public object GetService(Type serviceType)
        {
            try
            {
                if (serviceType == typeof(PersonasController))
                {
                    _context = new PersonasDbContext();
                    _personaRepository = new PersonaRepository(_context);
                    var mediator = new SimpleMediator(_personaRepository);
                    return new PersonasController(mediator);
                }
                // NUEVO: Resolver AuthController
                else if (serviceType == typeof(AuthController))
                {
                    _context = new PersonasDbContext();
                    _usuarioRepository = new UsuarioRepository(_context);
                    var tokenService = new JwtTokenService();
                    var mediator = new SimpleMediator(null, _usuarioRepository, tokenService);
                    return new AuthController(mediator);
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    string.Format("Error en GetService: {0}", ex.Message));
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
        private readonly IPersonaRepository _personaRepository;
        private readonly IUsuarioRepository _usuarioRepository;  // NUEVO
        private readonly ITokenService _tokenService;            // NUEVO

        public SimpleMediator(
            IPersonaRepository personaRepository,
            IUsuarioRepository usuarioRepository = null,
            ITokenService tokenService = null)
        {
            _personaRepository = personaRepository; // Puede ser null para AuthController
            _usuarioRepository = usuarioRepository;
            _tokenService = tokenService;
        }

        public async System.Threading.Tasks.Task<TResponse> Send<TResponse>(
            IRequest<TResponse> request, 
            System.Threading.CancellationToken cancellationToken = default)
        {
            // COMANDOS EXISTENTES DE PERSONA
            if (request is CrearPersonaCommand crearCmd)
            {
                var handler = new CrearPersonaCommandHandler(_personaRepository);
                var result = await handler.Handle(crearCmd, cancellationToken);
                return (TResponse)(object)result;
            }
            else if (request is ActualizarPersonaCommand actualizarCmd)
            {
                var handler = new ActualizarPersonaCommandHandler(_personaRepository);
                var result = await handler.Handle(actualizarCmd, cancellationToken);
                return (TResponse)(object)result;
            }
            else if (request is EliminarPersonaCommand eliminarCmd)
            {
                var handler = new EliminarPersonaCommandHandler(_personaRepository);
                var result = await handler.Handle(eliminarCmd, cancellationToken);
                return (TResponse)(object)result;
            }
            // NUEVO: Comando de autenticación
            else if (request is AutenticarUsuarioCommand autenticarCmd)
            {
                if (_usuarioRepository == null || _tokenService == null)
                    throw new InvalidOperationException("Dependencias de autenticación no configuradas");

                var handler = new AutenticarUsuarioCommandHandler(_usuarioRepository, _tokenService);
                var result = await handler.Handle(autenticarCmd, cancellationToken);
                return (TResponse)(object)result;
            }
            // QUERIES EXISTENTES DE PERSONA
            else if (request is ObtenerTodasPersonasQuery todasQuery)
            {
                var handler = new ObtenerTodasPersonasQueryHandler(_personaRepository);
                var result = await handler.Handle(todasQuery, cancellationToken);
                return (TResponse)(object)result;
            }
            else if (request is ObtenerPersonaPorIdQuery porIdQuery)
            {
                var handler = new ObtenerPersonaPorIdQueryHandler(_personaRepository);
                var result = await handler.Handle(porIdQuery, cancellationToken);
                return (TResponse)(object)result;
            }
            else if (request is ObtenerMedicosQuery medicosQuery)
            {
                var handler = new ObtenerMedicosQueryHandler(_personaRepository);
                var result = await handler.Handle(medicosQuery, cancellationToken);
                return (TResponse)(object)result;
            }
            else if (request is ObtenerPacientesQuery pacientesQuery)
            {
                var handler = new ObtenerPacientesQueryHandler(_personaRepository);
                var result = await handler.Handle(pacientesQuery, cancellationToken);
                return (TResponse)(object)result;
            }

            throw new NotSupportedException(
                string.Format("No se encontró handler para: {0}", request.GetType().Name));
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
