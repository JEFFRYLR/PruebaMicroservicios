using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Personas.Domain.Enums;

namespace Personas.Domain.ValueObjects
{
    public class Documento
    {
        public TipoDocumento TipoDocumento { get; private set; }

        public string Numero { get; private set; }

        protected Documento() { }

        public Documento(TipoDocumento tipoDocumento, string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
                throw new ArgumentException("El número de documento es obligatorio");

            TipoDocumento = tipoDocumento;
            Numero = numero;
        }
    }
}