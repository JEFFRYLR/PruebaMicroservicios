using System;

namespace Recetas.Domain.Entities
{
    /// <summary>
    /// Entity - Detalle de medicamento en una receta
    /// </summary>
    public class DetalleReceta
    {
        public int Id { get; private set; }
        
        /// <summary>
        /// ID de la receta a la que pertenece
        /// </summary>
        public int RecetaId { get; private set; }
        
        /// <summary>
        /// Nombre del medicamento
        /// </summary>
        public string NombreMedicamento { get; private set; }
        
        /// <summary>
        /// Dosis (ej: 500mg, 10ml)
        /// </summary>
        public string Dosis { get; private set; }
        
        /// <summary>
        /// Frecuencia de administración (ej: Cada 8 horas)
        /// </summary>
        public string Frecuencia { get; private set; }
        
        /// <summary>
        /// Duración del tratamiento (ej: 7 días, 2 semanas)
        /// </summary>
        public string Duracion { get; private set; }
        
        /// <summary>
        /// Indicaciones especiales
        /// </summary>
        public string Indicaciones { get; private set; }

        /// <summary>
        /// Constructor privado para EF
        /// </summary>
        private DetalleReceta() { }

        /// <summary>
        /// Constructor para agregar medicamento a receta
        /// </summary>
        public DetalleReceta(int recetaId, string nombreMedicamento, string dosis, 
                           string frecuencia, string duracion, string indicaciones = null)
        {
            ValidarRecetaId(recetaId);
            ValidarNombreMedicamento(nombreMedicamento);
            ValidarDosis(dosis);
            ValidarFrecuencia(frecuencia);
            ValidarDuracion(duracion);

            RecetaId = recetaId;
            NombreMedicamento = nombreMedicamento;
            Dosis = dosis;
            Frecuencia = frecuencia;
            Duracion = duracion;
            Indicaciones = indicaciones;
        }

        #region Validaciones

        private void ValidarRecetaId(int recetaId)
        {
            if (recetaId <= 0)
                throw new ArgumentException("El ID de la receta debe ser mayor a 0");
        }

        private void ValidarNombreMedicamento(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre del medicamento es obligatorio");

            if (nombre.Length > 200)
                throw new ArgumentException("El nombre del medicamento no puede exceder 200 caracteres");
        }

        private void ValidarDosis(string dosis)
        {
            if (string.IsNullOrWhiteSpace(dosis))
                throw new ArgumentException("La dosis es obligatoria");

            if (dosis.Length > 100)
                throw new ArgumentException("La dosis no puede exceder 100 caracteres");
        }

        private void ValidarFrecuencia(string frecuencia)
        {
            if (string.IsNullOrWhiteSpace(frecuencia))
                throw new ArgumentException("La frecuencia es obligatoria");

            if (frecuencia.Length > 100)
                throw new ArgumentException("La frecuencia no puede exceder 100 caracteres");
        }

        private void ValidarDuracion(string duracion)
        {
            if (string.IsNullOrWhiteSpace(duracion))
                throw new ArgumentException("La duración es obligatoria");

            if (duracion.Length > 100)
                throw new ArgumentException("La duración no puede exceder 100 caracteres");
        }

        #endregion
    }
}
