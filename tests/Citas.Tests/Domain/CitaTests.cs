using NUnit.Framework;
using Citas.Domain.Entities;
using Citas.Domain.Enums;
using System;

namespace Citas.Tests.Domain
{
    [TestFixture]
    public class CitaTests
    {
        [Test]
        public void Cita_ConDatosValidos_DeberiaCrearse()
        {
            // Arrange
            var fechaFutura = DateTime.Now.AddDays(5);
            var lugar = "Consultorio 101";
            var medicoId = 1;
            var pacienteId = 2;
            var motivo = "Consulta general";

            // Act
            var cita = new Cita(fechaFutura, lugar, medicoId, pacienteId, motivo);

            // Assert
            Assert.IsNotNull(cita);
            Assert.AreEqual(fechaFutura, cita.FechaCita);
            Assert.AreEqual(lugar, cita.Lugar);
            Assert.AreEqual(medicoId, cita.MedicoId);
            Assert.AreEqual(pacienteId, cita.PacienteId);
            Assert.AreEqual(motivo, cita.Motivo);
            Assert.AreEqual(EstadoCita.Pendiente, cita.Estado);
        }

        [Test]
        public void Cita_ConFechaPasada_DeberiaLanzarExcepcion()
        {
            // Arrange
            var fechaPasada = DateTime.Now.AddDays(-1);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                new Cita(fechaPasada, "Consultorio 1", 1, 2, "Motivo"));
        }

        [Test]
        public void Cita_ConLugarVacio_DeberiaLanzarExcepcion()
        {
            // Arrange
            var fechaFutura = DateTime.Now.AddDays(1);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                new Cita(fechaFutura, "", 1, 2, "Motivo"));
        }

        [Test]
        public void Cita_ConMedicoIdCero_DeberiaLanzarExcepcion()
        {
            // Arrange
            var fechaFutura = DateTime.Now.AddDays(1);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                new Cita(fechaFutura, "Consultorio 1", 0, 2, "Motivo"));
        }

        [Test]
        public void Cita_ConPacienteIdNegativo_DeberiaLanzarExcepcion()
        {
            // Arrange
            var fechaFutura = DateTime.Now.AddDays(1);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                new Cita(fechaFutura, "Consultorio 1", 1, -1, "Motivo"));
        }

        [Test]
        public void Cita_ConMotivoVacio_DeberiaLanzarExcepcion()
        {
            // Arrange
            var fechaFutura = DateTime.Now.AddDays(1);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                new Cita(fechaFutura, "Consultorio 1", 1, 2, ""));
        }

        [Test]
        public void IniciarAtencion_ConCitaPendiente_DeberiaCambiarAEnProceso()
        {
            // Arrange
            var cita = new Cita(DateTime.Now.AddDays(1), "Consultorio 1", 1, 2, "Consulta");

            // Act
            cita.IniciarAtencion();

            // Assert
            Assert.AreEqual(EstadoCita.EnProceso, cita.Estado);
            Assert.IsNotNull(cita.FechaActualizacion);
        }

        [Test]
        public void Finalizar_ConCitaEnProceso_DeberiaCambiarAFinalizada()
        {
            // Arrange
            var cita = new Cita(DateTime.Now.AddDays(1), "Consultorio 1", 1, 2, "Consulta");
            cita.IniciarAtencion();

            // Act
            cita.Finalizar();

            // Assert
            Assert.AreEqual(EstadoCita.Finalizada, cita.Estado);
            Assert.IsNotNull(cita.FechaActualizacion);
        }

        [Test]
        public void Finalizar_ConCitaPendiente_DeberiaLanzarExcepcion()
        {
            // Arrange
            var cita = new Cita(DateTime.Now.AddDays(1), "Consultorio 1", 1, 2, "Consulta");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => cita.Finalizar());
        }

        [Test]
        public void Cancelar_ConCitaPendiente_DeberiaCambiarACancelada()
        {
            // Arrange
            var cita = new Cita(DateTime.Now.AddDays(1), "Consultorio 1", 1, 2, "Consulta");

            // Act
            cita.Cancelar();

            // Assert
            Assert.AreEqual(EstadoCita.Cancelada, cita.Estado);
            Assert.IsNotNull(cita.FechaActualizacion);
        }

        [Test]
        public void Cancelar_ConCitaFinalizada_DeberiaLanzarExcepcion()
        {
            // Arrange
            var cita = new Cita(DateTime.Now.AddDays(1), "Consultorio 1", 1, 2, "Consulta");
            cita.IniciarAtencion();
            cita.Finalizar();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => cita.Cancelar());
        }

        [Test]
        public void ActualizarEstado_DePendienteAEnProceso_DeberiaActualizar()
        {
            // Arrange
            var cita = new Cita(DateTime.Now.AddDays(1), "Consultorio 1", 1, 2, "Consulta");

            // Act
            cita.ActualizarEstado(EstadoCita.EnProceso);

            // Assert
            Assert.AreEqual(EstadoCita.EnProceso, cita.Estado);
            Assert.IsNotNull(cita.FechaActualizacion);
        }

        [Test]
        public void ActualizarEstado_DesdeFinalizada_DeberiaLanzarExcepcion()
        {
            // Arrange
            var cita = new Cita(DateTime.Now.AddDays(1), "Consultorio 1", 1, 2, "Consulta");
            cita.IniciarAtencion();
            cita.Finalizar();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => 
                cita.ActualizarEstado(EstadoCita.Pendiente));
        }
    }
}
