using MediatR;
using Recetas.Application.Commands;
using Recetas.Application.Queries;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Recetas.API.Controllers
{
    /// <summary>
    /// API Controller para Recetas Médicas usando CQRS + MediatR
    /// </summary>
    [RoutePrefix("api/recetas")]
    public class RecetasController : ApiController
    {
        private readonly IMediator _mediator;

        public RecetasController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Obtiene información de la API
        /// </summary>
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok(new
            {
                mensaje = "API de Recetas funcionando con CQRS + MediatR",
                version = "1.0",
                patron = "CQRS (Command Query Responsibility Segregation)",
                endpoints = new[]
                {
                    "GET /api/recetas - Info de la API",
                    "POST /api/recetas - Crear receta con medicamentos",
                    "GET /api/recetas/{id} - Obtener receta por ID",
                    "GET /api/recetas/paciente/{pacienteId} - Recetas de un paciente",
                    "GET /api/recetas/medico/{medicoId} - Recetas emitidas por médico",
                    "GET /api/recetas/cita/{citaId} - Receta de una cita",
                    "POST /api/recetas/{id}/medicamento - Agregar medicamento a receta"
                }
            });
        }

        /// <summary>
        /// Crea una nueva receta con medicamentos - COMANDO CQRS
        /// </summary>
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Crear([FromBody] CrearRecetaCommand command)
        {
            try
            {
                if (command == null)
                    return BadRequest("El comando es nulo");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var recetaId = await _mediator.Send(command);

                return Ok(new
                {
                    mensaje = "Receta creada exitosamente",
                    id = recetaId
                });
            }
            catch (ArgumentException argEx)
            {
                return BadRequest($"Error de validación: {argEx.Message}");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new
                {
                    mensaje = ex.Message,
                    tipo = ex.GetType().Name
                });
            }
        }

        /// <summary>
        /// Obtiene una receta por ID - QUERY CQRS
        /// </summary>
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> ObtenerPorId(int id)
        {
            try
            {
                var query = new ObtenerRecetaPorIdQuery(id);
                var receta = await _mediator.Send(query);

                if (receta == null)
                    return NotFound();

                return Ok(receta);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtiene recetas de un paciente - QUERY CQRS
        /// </summary>
        [HttpGet]
        [Route("paciente/{pacienteId:int}")]
        public async Task<IHttpActionResult> ObtenerPorPaciente(int pacienteId)
        {
            try
            {
                var query = new ObtenerRecetasPorPacienteQuery(pacienteId);
                var recetas = await _mediator.Send(query);

                return Ok(recetas);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtiene recetas emitidas por un médico - QUERY CQRS
        /// </summary>
        [HttpGet]
        [Route("medico/{medicoId:int}")]
        public async Task<IHttpActionResult> ObtenerPorMedico(int medicoId)
        {
            try
            {
                var query = new ObtenerRecetasPorMedicoQuery(medicoId);
                var recetas = await _mediator.Send(query);

                return Ok(recetas);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtiene la receta de una cita específica - QUERY CQRS
        /// </summary>
        [HttpGet]
        [Route("cita/{citaId:int}")]
        public async Task<IHttpActionResult> ObtenerPorCita(int citaId)
        {
            try
            {
                var query = new ObtenerRecetaPorCitaQuery(citaId);
                var receta = await _mediator.Send(query);

                if (receta == null)
                    return NotFound();

                return Ok(receta);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Agrega un medicamento a una receta existente - COMANDO CQRS
        /// </summary>
        [HttpPost]
        [Route("{id:int}/medicamento")]
        public async Task<IHttpActionResult> AgregarMedicamento(int id, [FromBody] AgregarMedicamentoCommand command)
        {
            try
            {
                if (command == null)
                    return BadRequest("El comando es nulo");

                command.RecetaId = id;

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _mediator.Send(command);

                return Ok(new { mensaje = "Medicamento agregado exitosamente" });
            }
            catch (ArgumentException argEx)
            {
                return BadRequest($"Error de validación: {argEx.Message}");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
