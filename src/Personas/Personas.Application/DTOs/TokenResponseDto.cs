using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personas.Application.DTOs
{
    public class TokenResponseDto
    {
        public string Token { get; set; }
        public string NombreUsuario { get; set; }
        public int PersonaId { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string TipoPersona { get; set; }
        public int ExpiresIn { get; set; }
    }
}
