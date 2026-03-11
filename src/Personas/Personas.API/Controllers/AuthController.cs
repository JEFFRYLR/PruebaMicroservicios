using MediatR;
using Personas.Application.Commands;
using Personas.Application.DTOs;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Personas.API.Controllers
{
    /// <summary>
    /// Controlador de autenticación JWT
    /// </summary>
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Endpoint de login - Genera JWT token
        /// POST /api/auth/login
        /// </summary>
        /// <param name="request">Credenciales del usuario</param>
        /// <returns>Token JWT y datos del usuario</returns>
        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (request == null)
                return BadRequest("La petición no puede ser nula");

            if (string.IsNullOrWhiteSpace(request.NombreUsuario))
                return BadRequest("El nombre de usuario es requerido");

            if (string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("La contraseña es requerida");

            try
            {
                var command = new AutenticarUsuarioCommand
                {
                    NombreUsuario = request.NombreUsuario,
                    Password = request.Password
                };

                var response = await _mediator.Send(command);
                
                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    string.Format("Error en login: {0}", ex.Message));
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Endpoint de prueba para verificar autenticación
        /// GET /api/auth/me
        /// </summary>
        /// <returns>Información del usuario autenticado</returns>
        [HttpGet]
        [Route("me")]
        [Authorize]
        public IHttpActionResult GetCurrentUser()
        {
            var identity = User.Identity as System.Security.Claims.ClaimsIdentity;
            
            if (identity == null)
                return Unauthorized();

            var claims = identity.Claims.Select(c => new
            {
                type = c.Type,
                value = c.Value
            });

            return Ok(new
            {
                autenticado = User.Identity.IsAuthenticated,
                usuario = User.Identity.Name,
                claims = claims
            });
        }
    }
}
