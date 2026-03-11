using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Personas.Domain.Entities;

namespace Personas.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerarToken(Usuario usuario, Persona persona);
    }
}
