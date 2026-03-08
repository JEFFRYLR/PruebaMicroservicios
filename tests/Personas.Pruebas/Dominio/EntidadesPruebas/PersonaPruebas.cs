using FluentAssertions;
using NUnit.Framework;
using Personas.Domain.Entities;
using Personas.Domain.Enums;
using Personas.Domain.ValueObjects;
using System;

namespace Personas.Pruebas.Dominio.EntidadesPruebas
{
    /// <summary>
    /// Tests unitarios para la entidad Persona (Aggregate Root)
    /// Valida invariantes de negocio y comportamiento del dominio
    /// </summary>
    [TestFixture]
    public class PersonaPruebas
    {
        #region Constructor - Casos Válidos

        [Test]
        public void Persona_ConDatosValidos_DeberiaCrearse()
        {
            // ─── ARRANGE ───────────────────────────────────────
            var documento = new Documento(TipoDocumento.Cedula, "12345678");

            // ─── ACT ───────────────────────────────────────────
            var persona = new Persona("Juan", "Pérez", documento, TipoPersona.Medico);

            // ─── ASSERT ────────────────────────────────────────
            persona.Should().NotBeNull();
            persona.Nombre.Should().Be("Juan");
            persona.Apellido.Should().Be("Pérez");
            persona.Documento.Should().Be(documento);
            persona.TipoPersona.Should().Be(TipoPersona.Medico);
        }

        [Test]
        public void Persona_ConTipoPaciente_DeberiaCrearsePaciente()
        {
            // ─── ARRANGE ───────────────────────────────────────
            var documento = new Documento(TipoDocumento.Pasaporte, "ABC123456");

            // ─── ACT ───────────────────────────────────────────
            var persona = new Persona("María", "González", documento, TipoPersona.Paciente);

            // ─── ASSERT ────────────────────────────────────────
            persona.TipoPersona.Should().Be(TipoPersona.Paciente);
            persona.Nombre.Should().Be("María");
        }

        #endregion

        #region Constructor - Validaciones

        [Test]
        public void Persona_ConNombreVacio_DeberiaLanzarExcepcion()
        {
            // ─── ARRANGE ───────────────────────────────────────
            var documento = new Documento(TipoDocumento.Cedula, "12345678");

            // ─── ACT & ASSERT ──────────────────────────────────
            Action crearPersona = () => new Persona("", "Pérez", documento, TipoPersona.Medico);

            crearPersona.Should()
                .Throw<ArgumentException>()
                .WithMessage("*nombre*");
        }

        [Test]
        public void Persona_ConNombreNull_DeberiaLanzarExcepcion()
        {
            // ─── ARRANGE ───────────────────────────────────────
            var documento = new Documento(TipoDocumento.Cedula, "12345678");

            // ─── ACT & ASSERT ──────────────────────────────────
            Action crearPersona = () => new Persona(null, "Pérez", documento, TipoPersona.Medico);

            crearPersona.Should()
                .Throw<ArgumentException>()
                .WithMessage("*nombre*");
        }

        [Test]
        public void Persona_ConNombreSoloEspacios_DeberiaLanzarExcepcion()
        {
            // ─── ARRANGE ───────────────────────────────────────
            var documento = new Documento(TipoDocumento.Cedula, "12345678");

            // ─── ACT & ASSERT ──────────────────────────────────
            Action crearPersona = () => new Persona("   ", "Pérez", documento, TipoPersona.Medico);

            crearPersona.Should()
                .Throw<ArgumentException>()
                .WithMessage("*nombre*");
        }

        [Test]
        public void Persona_ConApellidoVacio_DeberiaLanzarExcepcion()
        {
            // ─── ARRANGE ───────────────────────────────────────
            var documento = new Documento(TipoDocumento.Cedula, "12345678");

            // ─── ACT & ASSERT ──────────────────────────────────
            Action crearPersona = () => new Persona("Juan", "", documento, TipoPersona.Medico);

            crearPersona.Should()
                .Throw<ArgumentException>()
                .WithMessage("*apellido*");
        }

        [Test]
        public void Persona_ConApellidoNull_DeberiaLanzarExcepcion()
        {
            // ─── ARRANGE ───────────────────────────────────────
            var documento = new Documento(TipoDocumento.Cedula, "12345678");

            // ─── ACT & ASSERT ──────────────────────────────────
            Action crearPersona = () => new Persona("Juan", null, documento, TipoPersona.Medico);

            crearPersona.Should()
                .Throw<ArgumentException>()
                .WithMessage("*apellido*");
        }

        [Test]
        public void Persona_ConDocumentoNull_DeberiaLanzarExcepcion()
        {
            // ─── ACT & ASSERT ──────────────────────────────────
            Action crearPersona = () => new Persona("Juan", "Pérez", null, TipoPersona.Medico);

            crearPersona.Should()
                .Throw<ArgumentException>()
                .WithMessage("*documento*");
        }

        #endregion

        #region Método Actualizar

        [Test]
        public void Persona_Actualizar_DeberiaModificarDatos()
        {
            // ─── ARRANGE ───────────────────────────────────────
            var documentoOriginal = new Documento(TipoDocumento.Cedula, "12345678");
            var persona = new Persona("Juan", "Pérez", documentoOriginal, TipoPersona.Medico);

            var nuevoDocumento = new Documento(TipoDocumento.Pasaporte, "ABC123456");

            // ─── ACT ───────────────────────────────────────────
            persona.Actualizar("Carlos", "López", nuevoDocumento, TipoPersona.Paciente);

            // ─── ASSERT ────────────────────────────────────────
            persona.Nombre.Should().Be("Carlos");
            persona.Apellido.Should().Be("López");
            persona.Documento.Should().Be(nuevoDocumento);
            persona.Documento.Numero.Should().Be("ABC123456");
            persona.TipoPersona.Should().Be(TipoPersona.Paciente);
        }

        [Test]
        public void Persona_ActualizarConNombreVacio_DeberiaLanzarExcepcion()
        {
            // ─── ARRANGE ───────────────────────────────────────
            var documento = new Documento(TipoDocumento.Cedula, "12345678");
            var persona = new Persona("Juan", "Pérez", documento, TipoPersona.Medico);

            var nuevoDocumento = new Documento(TipoDocumento.Pasaporte, "ABC123");

            // ─── ACT & ASSERT ──────────────────────────────────
            Action actualizar = () => persona.Actualizar("", "López", nuevoDocumento, TipoPersona.Medico);

            actualizar.Should()
                .Throw<ArgumentException>()
                .WithMessage("*nombre*");
        }

        [Test]
        public void Persona_ActualizarConApellidoVacio_DeberiaLanzarExcepcion()
        {
            // ─── ARRANGE ───────────────────────────────────────
            var documento = new Documento(TipoDocumento.Cedula, "12345678");
            var persona = new Persona("Juan", "Pérez", documento, TipoPersona.Medico);

            var nuevoDocumento = new Documento(TipoDocumento.Pasaporte, "ABC123");

            // ─── ACT & ASSERT ──────────────────────────────────
            Action actualizar = () => persona.Actualizar("Carlos", "", nuevoDocumento, TipoPersona.Medico);

            actualizar.Should()
                .Throw<ArgumentException>()
                .WithMessage("*apellido*");
        }

        [Test]
        public void Persona_ActualizarConDocumentoNull_DeberiaLanzarExcepcion()
        {
            // ─── ARRANGE ───────────────────────────────────────
            var documento = new Documento(TipoDocumento.Cedula, "12345678");
            var persona = new Persona("Juan", "Pérez", documento, TipoPersona.Medico);

            // ─── ACT & ASSERT ──────────────────────────────────
            Action actualizar = () => persona.Actualizar("Carlos", "López", null, TipoPersona.Medico);

            actualizar.Should()
                .Throw<ArgumentException>()
                .WithMessage("*documento*");
        }

        #endregion

        #region Pruebas de Invariantes

        [Test]
        public void Persona_AlCrearse_DeberiaTenerIdCero()
        {
            // ─── ARRANGE & ACT ─────────────────────────────────
            var documento = new Documento(TipoDocumento.Cedula, "12345678");
            var persona = new Persona("Juan", "Pérez", documento, TipoPersona.Medico);

            // ─── ASSERT ────────────────────────────────────────
            persona.Id.Should().Be(0);
        }

        [Test]
        public void Persona_AlCrearse_DeberiaTenerDocumentoAsignado()
        {
            // ─── ARRANGE & ACT ─────────────────────────────────
            var documento = new Documento(TipoDocumento.Cedula, "12345678");
            var persona = new Persona("Juan", "Pérez", documento, TipoPersona.Medico);

            // ─── ASSERT ────────────────────────────────────────
            persona.Documento.Should().NotBeNull();
            persona.Documento.TipoDocumento.Should().Be(TipoDocumento.Cedula);
            persona.Documento.Numero.Should().Be("12345678");
        }

        #endregion
    }
}
