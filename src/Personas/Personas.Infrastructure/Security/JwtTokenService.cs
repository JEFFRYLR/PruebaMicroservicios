using Microsoft.IdentityModel.Tokens;
using Personas.Application.Interfaces;
using Personas.Domain.Entities;
using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Personas.Infrastructure.Security
{
    public class JwtTokenService : ITokenService
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expirationMinutes;

        public JwtTokenService()
        {
            _secretKey = ConfigurationManager.AppSettings["JwtSecretKey"];
            _issuer = ConfigurationManager.AppSettings["JwtIssuer"];
            _audience = ConfigurationManager.AppSettings["JwtAudience"];
            
            var expirationConfig = ConfigurationManager.AppSettings["JwtExpirationMinutes"];
            _expirationMinutes = string.IsNullOrEmpty(expirationConfig) ? 60 : int.Parse(expirationConfig);

            if (string.IsNullOrEmpty(_secretKey))
                throw new InvalidOperationException("JwtSecretKey no está configurada en Web.config");
        }

        public string GenerarToken(Usuario usuario, Persona persona)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));
            
            if (persona == null)
                throw new ArgumentNullException(nameof(persona));

            // Crear clave de seguridad
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Definir claims (información del usuario en el token)
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.NombreUsuario),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UsuarioId", usuario.Id.ToString()),
                new Claim("PersonaId", persona.Id.ToString()),
                new Claim("TipoPersona", persona.TipoPersona.ToString()),
                new Claim(ClaimTypes.Name, string.Format("{0} {1}", persona.Nombre, persona.Apellido)),
                new Claim(ClaimTypes.Role, 
                    persona.TipoPersona == Domain.Enums.TipoPersona.Medico ? "Medico" : "Paciente")
            };

            // Crear token JWT
            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_expirationMinutes),
                signingCredentials: credentials
            );

            // Serializar token a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
