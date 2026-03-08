using NUnit.Framework;
using Recetas.Domain.Entities;
using System;

namespace Recetas.Tests.Domain
{
    [TestFixture]
    public class DetalleRecetaTests
    {
        [Test]
        public void DetalleReceta_ConDatosValidos_DeberiaCrearse()
        {
            var detalle = new DetalleReceta(1, "Paracetamol", "500mg", "Cada 8 horas", "5 días", "Tomar con alimentos");

            Assert.IsNotNull(detalle);
            Assert.AreEqual(1, detalle.RecetaId);
            Assert.AreEqual("Paracetamol", detalle.NombreMedicamento);
            Assert.AreEqual("500mg", detalle.Dosis);
            Assert.AreEqual("Cada 8 horas", detalle.Frecuencia);
            Assert.AreEqual("5 días", detalle.Duracion);
            Assert.AreEqual("Tomar con alimentos", detalle.Indicaciones);
        }

        [Test]
        public void DetalleReceta_SinIndicaciones_DeberiaCrearse()
        {
            var detalle = new DetalleReceta(1, "Ibuprofeno", "400mg", "Cada 12 horas", "3 días");

            Assert.IsNotNull(detalle);
            Assert.IsNull(detalle.Indicaciones);
        }

        [Test]
        public void DetalleReceta_ConRecetaIdCero_DeberiaLanzarExcepcion()
        {
            var ex = Assert.Throws<ArgumentException>(() => 
                new DetalleReceta(0, "Medicamento", "100mg", "Cada 8 horas", "5 días"));
            
            Assert.That(ex.Message, Does.Contain("ID de la receta"));
        }

        [Test]
        public void DetalleReceta_ConRecetaIdNegativo_DeberiaLanzarExcepcion()
        {
            var ex = Assert.Throws<ArgumentException>(() => 
                new DetalleReceta(-1, "Medicamento", "100mg", "Cada 8 horas", "5 días"));
            
            Assert.That(ex.Message, Does.Contain("ID de la receta"));
        }

        [Test]
        public void DetalleReceta_ConNombreMedicamentoVacio_DeberiaLanzarExcepcion()
        {
            var ex = Assert.Throws<ArgumentException>(() => 
                new DetalleReceta(1, "", "100mg", "Cada 8 horas", "5 días"));
            
            Assert.That(ex.Message, Does.Contain("nombre del medicamento"));
        }

        [Test]
        public void DetalleReceta_ConNombreMedicamentoNulo_DeberiaLanzarExcepcion()
        {
            var ex = Assert.Throws<ArgumentException>(() => 
                new DetalleReceta(1, null, "100mg", "Cada 8 horas", "5 días"));
            
            Assert.That(ex.Message, Does.Contain("nombre del medicamento"));
        }

        [Test]
        public void DetalleReceta_ConNombreMedicamentoMuyLargo_DeberiaLanzarExcepcion()
        {
            var nombreLargo = new string('A', 201);
            var ex = Assert.Throws<ArgumentException>(() => 
                new DetalleReceta(1, nombreLargo, "100mg", "Cada 8 horas", "5 días"));
            
            Assert.That(ex.Message, Does.Contain("200 caracteres"));
        }

        [Test]
        public void DetalleReceta_ConDosisVacia_DeberiaLanzarExcepcion()
        {
            var ex = Assert.Throws<ArgumentException>(() => 
                new DetalleReceta(1, "Medicamento", "", "Cada 8 horas", "5 días"));
            
            Assert.That(ex.Message, Does.Contain("dosis"));
        }

        [Test]
        public void DetalleReceta_ConDosisNula_DeberiaLanzarExcepcion()
        {
            var ex = Assert.Throws<ArgumentException>(() => 
                new DetalleReceta(1, "Medicamento", null, "Cada 8 horas", "5 días"));
            
            Assert.That(ex.Message, Does.Contain("dosis"));
        }

        [Test]
        public void DetalleReceta_ConDosisMuyLarga_DeberiaLanzarExcepcion()
        {
            var dosisLarga = new string('A', 101);
            var ex = Assert.Throws<ArgumentException>(() => 
                new DetalleReceta(1, "Medicamento", dosisLarga, "Cada 8 horas", "5 días"));
            
            Assert.That(ex.Message, Does.Contain("100 caracteres"));
        }

        [Test]
        public void DetalleReceta_ConFrecuenciaVacia_DeberiaLanzarExcepcion()
        {
            var ex = Assert.Throws<ArgumentException>(() => 
                new DetalleReceta(1, "Medicamento", "100mg", "", "5 días"));
            
            Assert.That(ex.Message, Does.Contain("frecuencia"));
        }

        [Test]
        public void DetalleReceta_ConFrecuenciaNula_DeberiaLanzarExcepcion()
        {
            var ex = Assert.Throws<ArgumentException>(() => 
                new DetalleReceta(1, "Medicamento", "100mg", null, "5 días"));
            
            Assert.That(ex.Message, Does.Contain("frecuencia"));
        }

        [Test]
        public void DetalleReceta_ConFrecuenciaMuyLarga_DeberiaLanzarExcepcion()
        {
            var frecuenciaLarga = new string('A', 101);
            var ex = Assert.Throws<ArgumentException>(() => 
                new DetalleReceta(1, "Medicamento", "100mg", frecuenciaLarga, "5 días"));
            
            Assert.That(ex.Message, Does.Contain("100 caracteres"));
        }

        [Test]
        public void DetalleReceta_ConDuracionVacia_DeberiaLanzarExcepcion()
        {
            var ex = Assert.Throws<ArgumentException>(() => 
                new DetalleReceta(1, "Medicamento", "100mg", "Cada 8 horas", ""));
            
            Assert.That(ex.Message, Does.Contain("duración"));
        }

        [Test]
        public void DetalleReceta_ConDuracionNula_DeberiaLanzarExcepcion()
        {
            var ex = Assert.Throws<ArgumentException>(() => 
                new DetalleReceta(1, "Medicamento", "100mg", "Cada 8 horas", null));
            
            Assert.That(ex.Message, Does.Contain("duración"));
        }

        [Test]
        public void DetalleReceta_ConDuracionMuyLarga_DeberiaLanzarExcepcion()
        {
            var duracionLarga = new string('A', 101);
            var ex = Assert.Throws<ArgumentException>(() => 
                new DetalleReceta(1, "Medicamento", "100mg", "Cada 8 horas", duracionLarga));
            
            Assert.That(ex.Message, Does.Contain("100 caracteres"));
        }

        [Test]
        public void DetalleReceta_ConDatosMinimoValidos_DeberiaCrearse()
        {
            var detalle = new DetalleReceta(1, "A", "1", "1", "1");

            Assert.IsNotNull(detalle);
            Assert.AreEqual("A", detalle.NombreMedicamento);
        }

        [Test]
        public void DetalleReceta_ConDatosMaximoValidos_DeberiaCrearse()
        {
            var nombreMaximo = new string('M', 200);
            var dosisMaxima = new string('D', 100);
            var frecuenciaMaxima = new string('F', 100);
            var duracionMaxima = new string('T', 100);

            var detalle = new DetalleReceta(1, nombreMaximo, dosisMaxima, frecuenciaMaxima, duracionMaxima);

            Assert.IsNotNull(detalle);
            Assert.AreEqual(200, detalle.NombreMedicamento.Length);
            Assert.AreEqual(100, detalle.Dosis.Length);
            Assert.AreEqual(100, detalle.Frecuencia.Length);
            Assert.AreEqual(100, detalle.Duracion.Length);
        }
    }
}
