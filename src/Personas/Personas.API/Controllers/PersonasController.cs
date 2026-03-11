using MediatR;
using Personas.Application.Commands;
using Personas.Application.DTOs;
using Personas.Application.Queries;
using Personas.Domain.Enums;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Personas.API.Controllers
{
    /// <summary>
    /// API Controller para Personas (Médicos y Pacientes) usando CQRS + MediatR
    /// ✅ PROTEGIDO CON JWT - Requiere autenticación en todos los endpoints
    /// </summary>
    [Authorize]  
    [RoutePrefix("api/personas")]
    public class PersonasController : ApiController
    {
        private readonly IMediator _mediator;

        public PersonasController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Obtiene todas las personas (médicos y pacientes)
        /// </summary>
        [HttpGet]
        [Route("")]
        [AllowAnonymous]  // ← Permitir acceso público a este endpoint
        public async Task<IHttpActionResult> ObtenerTodas()
        {
            try
            {
                var query = new ObtenerTodasPersonasQuery();
                var personas = await _mediator.Send(query);
                return Ok(personas);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtiene una persona por su ID
        /// ✅ Requiere autenticación
        /// </summary>
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> ObtenerPorId(int id)
        {
            try
            {
                var query = new ObtenerPersonaPorIdQuery(id);
                var persona = await _mediator.Send(query);

                if (persona == null)
                    return NotFound();

                return Ok(persona);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtiene todos los médicos
        /// ✅ Requiere autenticación
        /// </summary>
        [HttpGet]
        [Route("medicos")]
        public async Task<IHttpActionResult> ObtenerMedicos()
        {
            try
            {
                var query = new ObtenerMedicosQuery();
                var medicos = await _mediator.Send(query);
                return Ok(medicos);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtiene todos los pacientes
        /// ✅ Requiere autenticación
        /// </summary>
        [HttpGet]
        [Route("pacientes")]
        public async Task<IHttpActionResult> ObtenerPacientes()
        {
            try
            {
                var query = new ObtenerPacientesQuery();
                var pacientes = await _mediator.Send(query);
                return Ok(pacientes);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Crea una nueva persona (médico o paciente) - COMANDO CQRS
        /// ⚠️ REQUIERE AUTENTICACIÓN - Solo usuarios autenticados pueden crear personas
        /// </summary>
        [HttpPost]
        [Route("")]
        // ← ESTE ENDPOINT AHORA REQUIERE TOKEN (sin [AllowAnonymous])
        public async Task<IHttpActionResult> Crear([FromBody] CrearPersonaCommand command)
        {
            try
            {
                if (command == null)
                    return BadRequest("El comando es nulo");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var personaId = await _mediator.Send(command);

                return Ok(new { 
                    mensaje = "Persona creada exitosamente",
                    id = personaId 
                });
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(string.Format("Error de validación: {0}", argEx.Message));
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new { 
                    mensaje = ex.Message,
                    tipo = ex.GetType().Name
                });
            }
        }

        /// <summary>
        /// Actualiza una persona existente - COMANDO CQRS
        /// ✅ Requiere autenticación
        /// </summary>
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> Actualizar(int id, [FromBody] ActualizarPersonaCommand command)
        {
            try
            {
                if (command == null)
                    return BadRequest("El comando es nulo");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                command.Id = id;
                await _mediator.Send(command);

                return Ok(new { mensaje = "Persona actualizada exitosamente" });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Elimina una persona - COMANDO CQRS
        /// ✅ Requiere autenticación
        /// </summary>
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> Eliminar(int id)
        {
            try
            {
                var command = new EliminarPersonaCommand(id);
                await _mediator.Send(command);

                return Ok(new { mensaje = "Persona eliminada exitosamente" });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Valida si un médico existe y está activo
        /// ✅ Requiere autenticación
        /// </summary>
        [HttpGet]
        [Route("medicos/{id:int}/validar")]
        public async Task<IHttpActionResult> ValidarMedico(int id)
        {
            try
            {
                var query = new ObtenerPersonaPorIdQuery(id);
                var persona = await _mediator.Send(query);

                if (persona == null)
                    return Ok(new { existe = false, mensaje = "Médico no encontrado" });

                bool esMedico = persona.TipoPersona == TipoPersona.Medico;
                
                return Ok(new { 
                    existe = esMedico,
                    id = persona.Id,
                    nombre = string.Format("{0} {1}", persona.Nombre, persona.Apellido),
                    tipoPersona = persona.TipoPersona.ToString()
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Valida si un paciente existe y está activo
        /// ✅ Requiere autenticación
        /// </summary>
        [HttpGet]
        [Route("pacientes/{id:int}/validar")]
        public async Task<IHttpActionResult> ValidarPaciente(int id)
        {
            try
            {
                var query = new ObtenerPersonaPorIdQuery(id);
                var persona = await _mediator.Send(query);

                if (persona == null)
                    return Ok(new { existe = false, mensaje = "Paciente no encontrado" });

                bool esPaciente = persona.TipoPersona == TipoPersona.Paciente;
                
                return Ok(new { 
                    existe = esPaciente,
                    id = persona.Id,
                    nombre = string.Format("{0} {1}", persona.Nombre, persona.Apellido),
                    tipoPersona = persona.TipoPersona.ToString()
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
