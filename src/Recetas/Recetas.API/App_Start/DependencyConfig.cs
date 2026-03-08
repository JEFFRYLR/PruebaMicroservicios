using MediatR;
using Recetas.API.Controllers;
using Recetas.Application.CommandHandlers;
using Recetas.Application.Commands;
using Recetas.Application.DTOs;
using Recetas.Application.Queries;
using Recetas.Application.QueryHandlers;
using Recetas.Domain.Interfaces;
using Recetas.Infrastructure.Persistence;
using Recetas.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;

namespace Recetas.API.App_Start
{
    /// <summary>
    /// Configuración de Inyección de Dependencias con CQRS + MediatR
    /// </summary>
    public static class DependencyConfig
    {
        public static void Register(System.Web.Http.HttpConfiguration config)
        {
            config.DependencyResolver = new RecetasDependencyResolver();
        }
    }

    public class RecetasDependencyResolver : IDependencyResolver
    {
        public IDependencyScope BeginScope()
        {
            return new RecetasDependencyScope();
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

    public class RecetasDependencyScope : IDependencyScope
    {
        private RecetasDbContext _context;
        private IRecetaRepository _repository;

        public object GetService(Type serviceType)
        {
            try
            {
                if (serviceType == typeof(RecetasController))
                {
                    _context = new RecetasDbContext();
                    _repository = new RecetaRepository(_context);
                    var mediator = new SimpleMediator(_repository);
                    return new RecetasController(mediator);
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
    /// Implementación simple de IMediator para Recetas (CQRS Pattern)
    /// </summary>
    public class SimpleMediator : IMediator
    {
        private readonly IRecetaRepository _repository;

        public SimpleMediator(IRecetaRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async System.Threading.Tasks.Task<TResponse> Send<TResponse>(IRequest<TResponse> request, System.Threading.CancellationToken cancellationToken = default)
        {
            // COMMANDS
            if (request is CrearRecetaCommand crearCmd)
            {
                var handler = new CrearRecetaCommandHandler(_repository);
                var result = await handler.Handle(crearCmd, cancellationToken);
                return (TResponse)(object)result;
            }
            else if (request is AgregarMedicamentoCommand agregarCmd)
            {
                var handler = new AgregarMedicamentoCommandHandler(_repository);
                var result = await handler.Handle(agregarCmd, cancellationToken);
                return (TResponse)(object)result;
            }
            // QUERIES
            else if (request is ObtenerRecetaPorIdQuery porIdQuery)
            {
                var handler = new ObtenerRecetaPorIdQueryHandler(_repository);
                var result = await handler.Handle(porIdQuery, cancellationToken);
                return (TResponse)(object)result;
            }
            else if (request is ObtenerRecetasPorPacienteQuery porPacienteQuery)
            {
                var handler = new ObtenerRecetasPorPacienteQueryHandler(_repository);
                var result = await handler.Handle(porPacienteQuery, cancellationToken);
                return (TResponse)(object)result;
            }
            else if (request is ObtenerRecetasPorMedicoQuery porMedicoQuery)
            {
                var handler = new ObtenerRecetasPorMedicoQueryHandler(_repository);
                var result = await handler.Handle(porMedicoQuery, cancellationToken);
                return (TResponse)(object)result;
            }
            else if (request is ObtenerRecetaPorCitaQuery porCitaQuery)
            {
                var handler = new ObtenerRecetaPorCitaQueryHandler(_repository);
                var result = await handler.Handle(porCitaQuery, cancellationToken);
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
