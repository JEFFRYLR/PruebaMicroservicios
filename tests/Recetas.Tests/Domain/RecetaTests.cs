using NUnit.Framework;
using Recetas.Domain.Entities;
using System;

namespace Recetas.Tests.Domain
{
    [TestFixture]
    public class RecetaTests
    {
        [Test]
        public void Receta_ConDatosValidos_DeberiaCrearse()
        {
            var vigenciaFutura = DateTime.Now.AddDays(30);
            var receta = new Receta(1, 2, 3, "Gripe común", vigenciaFutura, "Reposo relativo");

            Assert.IsNotNull(receta);
            Assert.AreEqual(1, receta.CitaId);
            Assert.AreEqual(2, receta.MedicoId);
            Assert.AreEqual(3, receta.PacienteId);
            Assert.AreEqual("Gripe común", receta.Diagnostico);
            Assert.AreEqual(vigenciaFutura, receta.Vigencia);
            Assert.AreEqual("Reposo relativo", receta.Observaciones);
            Assert.IsNotNull(receta.FechaEmision);
        }

        [Test]
        public void Receta_ConCitaIdCero_DeberiaLanzarExcepcion()
        {
            var vigenciaFutura = DateTime.Now.AddDays(30);
            var ex = Assert.Throws<ArgumentException>(() => 
                new Receta(0, 1, 1, "Diagnóstico", vigenciaFutura));
            
            Assert.That(ex.Message, Does.Contain("ID de la cita"));
        }

        [Test]
        public void Receta_ConCitaIdNegativo_DeberiaLanzarExcepcion()
        {
            var vigenciaFutura = DateTime.Now.AddDays(30);
            var ex = Assert.Throws<ArgumentException>(() => 
                new Receta(-1, 1, 1, "Diagnóstico", vigenciaFutura));
            
            Assert.That(ex.Message, Does.Contain("ID de la cita"));
        }

        [Test]
        public void Receta_ConMedicoIdCero_DeberiaLanzarExcepcion()
        {
            var vigenciaFutura = DateTime.Now.AddDays(30);
            var ex = Assert.Throws<ArgumentException>(() => 
                new Receta(1, 0, 1, "Diagnóstico", vigenciaFutura));
            
            Assert.That(ex.Message, Does.Contain("ID del médico"));
        }

        [Test]
        public void Receta_ConPacienteIdCero_DeberiaLanzarExcepcion()
        {
            var vigenciaFutura = DateTime.Now.AddDays(30);
            var ex = Assert.Throws<ArgumentException>(() => 
                new Receta(1, 1, 0, "Diagnóstico", vigenciaFutura));
            
            Assert.That(ex.Message, Does.Contain("ID del paciente"));
        }

        [Test]
        public void Receta_ConDiagnosticoVacio_DeberiaLanzarExcepcion()
        {
            var vigenciaFutura = DateTime.Now.AddDays(30);
            var ex = Assert.Throws<ArgumentException>(() => 
                new Receta(1, 1, 1, "", vigenciaFutura));
            
            Assert.That(ex.Message, Does.Contain("diagnóstico"));
        }

        [Test]
        public void Receta_ConDiagnosticoNulo_DeberiaLanzarExcepcion()
        {
            var vigenciaFutura = DateTime.Now.AddDays(30);
            var ex = Assert.Throws<ArgumentException>(() => 
                new Receta(1, 1, 1, null, vigenciaFutura));
            
            Assert.That(ex.Message, Does.Contain("diagnóstico"));
        }

        [Test]
        public void Receta_ConDiagnosticoMuyLargo_DeberiaLanzarExcepcion()
        {
            var vigenciaFutura = DateTime.Now.AddDays(30);
            var diagnosticoLargo = new string('A', 1001);
            
            var ex = Assert.Throws<ArgumentException>(() => 
                new Receta(1, 1, 1, diagnosticoLargo, vigenciaFutura));
            
            Assert.That(ex.Message, Does.Contain("1000 caracteres"));
        }

        [Test]
        public void Receta_ConVigenciaPasada_DeberiaLanzarExcepcion()
        {
            var vigenciaPasada = DateTime.Now.AddDays(-1);
            var ex = Assert.Throws<ArgumentException>(() => 
                new Receta(1, 1, 1, "Diagnóstico", vigenciaPasada));
            
            Assert.That(ex.Message, Does.Contain("vigencia"));
        }

        [Test]
        public void Receta_ConVigenciaActual_DeberiaLanzarExcepcion()
        {
            var vigenciaActual = DateTime.Now;
            var ex = Assert.Throws<ArgumentException>(() => 
                new Receta(1, 1, 1, "Diagnóstico", vigenciaActual));
            
            Assert.That(ex.Message, Does.Contain("vigencia"));
        }

        [Test]
        public void Receta_SinObservaciones_DeberiaCrearse()
        {
            var vigenciaFutura = DateTime.Now.AddDays(30);
            var receta = new Receta(1, 2, 3, "Diagnóstico test", vigenciaFutura);

            Assert.IsNotNull(receta);
            Assert.IsNull(receta.Observaciones);
        }

        [Test]
        public void AgregarMedicamento_ConDatosValidos_DeberiaAgregarse()
        {
            var vigenciaFutura = DateTime.Now.AddDays(30);
            var receta = new Receta(1, 2, 3, "Gripe", vigenciaFutura);

            receta.AgregarMedicamento("Paracetamol", "500mg", "Cada 8 horas", "5 días", "Tomar con alimentos");

            Assert.AreEqual(1, receta.Detalles.Count);
        }

        [Test]
        public void AgregarMedicamento_VariosItems_DeberiaTenerTodos()
        {
            var vigenciaFutura = DateTime.Now.AddDays(30);
            var receta = new Receta(1, 2, 3, "Infección respiratoria", vigenciaFutura);

            receta.AgregarMedicamento("Amoxicilina", "500mg", "Cada 8 horas", "7 días");
            receta.AgregarMedicamento("Ibuprofeno", "400mg", "Cada 12 horas", "3 días");
            receta.AgregarMedicamento("Loratadina", "10mg", "Cada 24 horas", "5 días");

            Assert.AreEqual(3, receta.Detalles.Count);
        }

        [Test]
        public void ActualizarDiagnostico_ConDatosValidos_DeberiaActualizar()
        {
            var vigenciaFutura = DateTime.Now.AddDays(30);
            var receta = new Receta(1, 2, 3, "Diagnóstico inicial", vigenciaFutura);

            receta.ActualizarDiagnostico("Diagnóstico actualizado", "Nuevas observaciones");

            Assert.AreEqual("Diagnóstico actualizado", receta.Diagnostico);
            Assert.AreEqual("Nuevas observaciones", receta.Observaciones);
        }

        [Test]
        public void ActualizarDiagnostico_ConDiagnosticoVacio_DeberiaLanzarExcepcion()
        {
            var vigenciaFutura = DateTime.Now.AddDays(30);
            var receta = new Receta(1, 2, 3, "Diagnóstico inicial", vigenciaFutura);

            var ex = Assert.Throws<ArgumentException>(() => 
                receta.ActualizarDiagnostico("", "Observaciones"));
            
            Assert.That(ex.Message, Does.Contain("diagnóstico"));
        }

        [Test]
        public void Detalles_DeberiaSerSoloLectura()
        {
            var vigenciaFutura = DateTime.Now.AddDays(30);
            var receta = new Receta(1, 2, 3, "Diagnóstico", vigenciaFutura);

            var detalles = receta.Detalles;

            Assert.IsNotNull(detalles);
            Assert.IsInstanceOf<System.Collections.ObjectModel.ReadOnlyCollection<DetalleReceta>>(detalles);
        }
    }
}
