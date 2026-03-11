using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Personas.Application.DTOs;

namespace Personas.Application.Commands
{
    public class AutenticarUsuarioCommand : IRequest<TokenResponseDto>
    {
        public string NombreUsuario { get; set; }
        public string Password { get; set; }
    }
}
