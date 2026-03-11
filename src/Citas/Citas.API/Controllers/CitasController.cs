using Citas.Application.Commands;
using Citas.Application.Queries;
using MediatR;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Citas.API.Controllers
{
    [Authorize]  // ← AGREGAR AQUÍ: Protege todo el controlador
    [RoutePrefix("api/citas")]
    public class CitasController : ApiController
    {
        private readonly IMediator _mediator;

        public CitasController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Endpoint de prueba
        /// GET /api/citas
        /// </summary>
        [HttpGet]
        [Route("")]
        [AllowAnonymous]  // ← AGREGAR: Permitir acceso público a este endpoint
        public IHttpActionResult Get()
        {
            return Ok(new
            {
                mensaje = "API de Citas con JWT Authentication",
                version = "3.0",
                patron = "CQRS (Command Query Responsibility Segregation)",
                integracion = "HTTP Sincrónico con Microservicio de Personas",
                endpoints = new[]
                {
                    "GET /api/citas - Info de la API",
                    "POST /api/citas - Agendar cita (Command con validación)",
                    "GET /api/citas/{id} - Obtener cita por ID (Query)",
                    "GET /api/citas/medico/{medicoId} - Citas por médico (Query)",
                    "GET /api/citas/paciente/{pacienteId} - Citas por paciente (Query)",
                    "GET /api/citas/pendientes - Citas pendientes (Query)",
                    "PUT /api/citas/{id}/estado - Actualizar estado (Command)",
                    "POST /api/citas/{id}/finalizar - Finalizar cita (Command + RabbitMQ)"
                }
            });
        }

        /// <summary>
        /// Agendar nueva cita con validación de médico y paciente
        /// POST /api/citas
        /// </summary>
        [HttpPost]
        [Route("")]
        // ← Este endpoint ahora requiere autenticación
        public async Task<IHttpActionResult> AgendarCita([FromBody] AgendarCitaCommand command)
        {
            if (command == null)
                return BadRequest("El comando no puede ser nulo");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // NUEVO: Extraer token del header para propagación
                var authHeader = Request.Headers.Authorization;
                if (authHeader != null && authHeader.Scheme == "Bearer")
                {
                    command.BearerToken = authHeader.Parameter;
                }

                var citaId = await _mediator.Send(command);
                
                return Ok(new 
                { 
                    id = citaId, 
                    mensaje = "Cita agendada exitosamente",
                    fechaCita = command.FechaCita,
                    lugar = command.Lugar
                });
            }
            catch (InvalidOperationException invalidEx)
            {
                return BadRequest(invalidEx.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtener cita por ID
        /// GET /api/citas/{id}
        /// </summary>
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> ObtenerPorId(int id)
        {
            try
            {
                var query = new ObtenerCitaPorIdQuery { CitaId = id };
                var cita = await _mediator.Send(query);

                if (cita == null)
                    return NotFound();

                return Ok(cita);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtener citas de un médico
        /// GET /api/citas/medico/{medicoId}
        /// </summary>
        [HttpGet]
        [Route("medico/{medicoId:int}")]
        public async Task<IHttpActionResult> ObtenerPorMedico(int medicoId)
        {
            try
            {
                var query = new ObtenerCitasPorMedicoQuery { MedicoId = medicoId };
                var citas = await _mediator.Send(query);
                return Ok(citas);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtener citas de un paciente
        /// GET /api/citas/paciente/{pacienteId}
        /// </summary>
        [HttpGet]
        [Route("paciente/{pacienteId:int}")]
        public async Task<IHttpActionResult> ObtenerPorPaciente(int pacienteId)
        {
            try
            {
                var query = new ObtenerCitasPorPacienteQuery { PacienteId = pacienteId };
                var citas = await _mediator.Send(query);
                return Ok(citas);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtener citas pendientes
        /// GET /api/citas/pendientes
        /// </summary>
        [HttpGet]
        [Route("pendientes")]
        public async Task<IHttpActionResult> ObtenerPendientes()
        {
            try
            {
                var query = new ObtenerCitasPendientesQuery();
                var citas = await _mediator.Send(query);
                return Ok(citas);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Actualizar estado de una cita
        /// PUT /api/citas/{id}/estado
        /// </summary>
        [HttpPut]
        [Route("{id:int}/estado")]
        public async Task<IHttpActionResult> ActualizarEstado(int id, [FromBody] ActualizarEstadoCitaCommand command)
        {
            if (command == null)
                return BadRequest("El comando no puede ser nulo");

            try
            {
                command.CitaId = id;
                await _mediator.Send(command);
                return Ok(new { mensaje = "Estado actualizado exitosamente" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Finalizar cita (emite evento a RabbitMQ)
        /// POST /api/citas/{id}/finalizar
        /// </summary>
        [HttpPost]
        [Route("{id:int}/finalizar")]
        public async Task<IHttpActionResult> FinalizarCita(int id)
        {
            try
            {
                var command = new FinalizarCitaCommand { CitaId = id };
                await _mediator.Send(command);
                return Ok(new { mensaje = "Cita finalizada. Se ha enviado solicitud de receta al médico." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
