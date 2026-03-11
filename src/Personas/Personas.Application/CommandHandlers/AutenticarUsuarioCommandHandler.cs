using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Personas.Application.Commands;
using Personas.Application.DTOs;
using Personas.Application.Interfaces;
using Personas.Domain.Interfaces;

namespace Personas.Application.CommandHandlers
{
    public class AutenticarUsuarioCommandHandler : IRequestHandler<AutenticarUsuarioCommand, TokenResponseDto>
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITokenService _tokenService;

        public AutenticarUsuarioCommandHandler(
            IUsuarioRepository usuarioRepository, 
            ITokenService tokenService)
        {
            _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        public async Task<TokenResponseDto> Handle(
            AutenticarUsuarioCommand request, 
            CancellationToken cancellationToken)
        {
            // Buscar usuario por nombre de usuario
            var usuario = await _usuarioRepository.ObtenerPorNombreUsuarioAsync(request.NombreUsuario);

            if (usuario == null || !usuario.Activo)
            {
                throw new UnauthorizedAccessException("Credenciales inválidas");
            }

            // Verificar password usando BCrypt
            bool passwordValido = BCrypt.Net.BCrypt.Verify(request.Password, usuario.PasswordHash);
            
            if (!passwordValido)
            {
                throw new UnauthorizedAccessException("Credenciales inválidas");
            }

            // Obtener persona asociada al usuario
            var persona = await _usuarioRepository.ObtenerPersonaPorIdAsync(usuario.PersonaId);
            
            if (persona == null)
            {
                throw new InvalidOperationException("Usuario sin persona asociada");
            }

            // Generar token JWT
            var token = _tokenService.GenerarToken(usuario, persona);

            // Retornar respuesta con token
            return new TokenResponseDto
            {
                Token = token,
                NombreUsuario = usuario.NombreUsuario,
                PersonaId = persona.Id,
                Nombre = persona.Nombre,
                Apellido = persona.Apellido,
                TipoPersona = persona.TipoPersona.ToString(),
                ExpiresIn = 3600 // 1 hora en segundos
            };
        }
    }
}
