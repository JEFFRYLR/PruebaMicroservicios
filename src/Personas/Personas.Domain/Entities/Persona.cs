using Personas.Domain.Enums;
using Personas.Domain.ValueObjects;
using System;

namespace Personas.Domain.Entities
{
    public class Persona
    {
        public int Id { get; private set; }

        public string Nombre { get; private set; }

        public string Apellido { get; private set; }

        public Documento Documento { get; private set; }

        public TipoPersona TipoPersona { get; private set; }

        protected Persona() { }

        public Persona(string nombre, string apellido, Documento documento, TipoPersona tipoPersona)
        {
            ValidarDatos(nombre, apellido, documento);

            Nombre = nombre;
            Apellido = apellido;
            Documento = documento;
            TipoPersona = tipoPersona;
        }

        public void Actualizar(string nombre, string apellido, Documento documento, TipoPersona tipoPersona)
        {
            ValidarDatos(nombre, apellido, documento);

            Nombre = nombre;
            Apellido = apellido;
            Documento = documento;
            TipoPersona = tipoPersona;
        }

        private void ValidarDatos(string nombre, string apellido, Documento documento)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre es obligatorio");

            if (string.IsNullOrWhiteSpace(apellido))
                throw new ArgumentException("El apellido es obligatorio");

            if (documento == null)
                throw new ArgumentException("El documento es obligatorio");
        }
    }
}
