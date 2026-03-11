using System;

namespace Personas.Domain.Entities
{
    /// <summary>
    /// Entidad de usuario para autenticación JWT
    /// </summary>
    public class Usuario
    {
        public int Id { get; private set; }
        public int PersonaId { get; private set; }
        public string NombreUsuario { get; private set; }
        public string PasswordHash { get; private set; }
        public bool Activo { get; private set; }
        public DateTime FechaCreacion { get; private set; }
        
        // Navegación
        public virtual Persona Persona { get; private set; }

        // Constructor para Entity Framework
        protected Usuario() { }

        // Constructor para crear nuevo usuario
        public Usuario(int personaId, string nombreUsuario, string passwordHash)
        {
            if (personaId <= 0)
                throw new ArgumentException("PersonaId inválido");
            
            if (string.IsNullOrWhiteSpace(nombreUsuario))
                throw new ArgumentException("Nombre de usuario requerido");
            
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash requerido");

            PersonaId = personaId;
            NombreUsuario = nombreUsuario;
            PasswordHash = passwordHash;
            Activo = true;
            FechaCreacion = DateTime.Now;
        }

        public void CambiarPassword(string nuevoPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(nuevoPasswordHash))
                throw new ArgumentException("Password hash requerido");
            
            PasswordHash = nuevoPasswordHash;
        }

        public void Desactivar()
        {
            Activo = false;
        }

        public void Activar()
        {
            Activo = true;
        }
    }
}
