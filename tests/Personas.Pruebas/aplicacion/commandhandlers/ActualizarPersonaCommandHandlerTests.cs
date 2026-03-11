using FluentAssertions;
using Moq;
using NUnit.Framework;
using Personas.Application.CommandHandlers;
using Personas.Application.Commands;
using Personas.Domain.Entities;
using Personas.Domain.Enums;
using Personas.Domain.Interfaces;
using Personas.Domain.ValueObjects;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Personas.Pruebas.Aplicacion.CommandHandlers
{
    [TestFixture]
    public class ActualizarPersonaCommandHandlerTests
    {
        private Mock<IPersonaRepository> _mockRepository;
        private ActualizarPersonaCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IPersonaRepository>();
            _handler = new ActualizarPersonaCommandHandler(_mockRepository.Object);
        }

        [Test]
        public async Task Handle_ConDatosValidos_LlamaActualizarUnaVez()
        {
            // Arrange
            var documento = new Documento(TipoDocumento.Cedula, "12345678");
            var personaExistente = new Persona("Juan", "Perez", documento, TipoPersona.Medico);

            var command = new ActualizarPersonaCommand
            {
                Id = 1,
                Nombre = "Carlos",
                Apellido = "Lopez",
                TipoDocumento = TipoDocumento.Pasaporte,
                NumeroDocumento = "87654321"
            };

            _mockRepository
                .Setup(x => x.ObtenerPorId(1))
                .Returns(personaExistente);

            _mockRepository
                .Setup(x => x.Actualizar(It.IsAny<Persona>()));

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockRepository.Verify(x => x.Actualizar(It.IsAny<Persona>()), Times.Once);
        }

        [Test]
        public async Task Handle_ConDatosValidos_LlamaObtenerPorIdUnaVez()
        {
            // Arrange
            var documento = new Documento(TipoDocumento.Cedula, "12345678");
            var personaExistente = new Persona("Juan", "Perez", documento, TipoPersona.Medico);

            var command = new ActualizarPersonaCommand
            {
                Id = 1,
                Nombre = "Carlos",
                Apellido = "Lopez",
                TipoDocumento = TipoDocumento.Pasaporte,
                NumeroDocumento = "87654321"
            };

            _mockRepository
                .Setup(x => x.ObtenerPorId(1))
                .Returns(personaExistente);

            _mockRepository
                .Setup(x => x.Actualizar(It.IsAny<Persona>()));

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockRepository.Verify(x => x.ObtenerPorId(1), Times.Once);
        }

        [Test]
        public void Handle_ConIdInexistente_LanzaException()
        {
            // Arrange
            var command = new ActualizarPersonaCommand
            {
                Id = 999,
                Nombre = "Juan",
                Apellido = "Perez",
                TipoDocumento = TipoDocumento.Cedula,
                NumeroDocumento = "12345678"
            };

            _mockRepository
                .Setup(x => x.ObtenerPorId(999))
                .Returns((Persona)null);

            // Act & Assert
            var ex = Assert.CatchAsync<Exception>(async () =>
            {
                await _handler.Handle(command, CancellationToken.None);
            });

            Assert.IsNotNull(ex);
        }

        [Test]
        public async Task Handle_ActualizaNombre_CambiaElNombre()
        {
            // Arrange
            var documento = new Documento(TipoDocumento.Cedula, "12345678");
            var personaExistente = new Persona("Juan", "Perez", documento, TipoPersona.Medico);

            var command = new ActualizarPersonaCommand
            {
                Id = 1,
                Nombre = "Pedro",
                Apellido = "Ramirez",
                TipoDocumento = TipoDocumento.Cedula,
                NumeroDocumento = "11111111"
            };

            Persona personaActualizada = null;
            _mockRepository
                .Setup(x => x.ObtenerPorId(1))
                .Returns(personaExistente);

            _mockRepository
                .Setup(x => x.Actualizar(It.IsAny<Persona>()))
                .Callback<Persona>(p => personaActualizada = p);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsNotNull(personaActualizada);
            Assert.AreEqual("Pedro", personaActualizada.Nombre);
        }

        [Test]
        public async Task Handle_ActualizaApellido_CambiaElApellido()
        {
            // Arrange
            var documento = new Documento(TipoDocumento.Cedula, "12345678");
            var personaExistente = new Persona("Juan", "Perez", documento, TipoPersona.Medico);

            var command = new ActualizarPersonaCommand
            {
                Id = 1,
                Nombre = "Pedro",
                Apellido = "Ramirez",
                TipoDocumento = TipoDocumento.Cedula,
                NumeroDocumento = "11111111"
            };

            Persona personaActualizada = null;
            _mockRepository
                .Setup(x => x.ObtenerPorId(1))
                .Returns(personaExistente);

            _mockRepository
                .Setup(x => x.Actualizar(It.IsAny<Persona>()))
                .Callback<Persona>(p => personaActualizada = p);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsNotNull(personaActualizada);
            Assert.AreEqual("Ramirez", personaActualizada.Apellido);
        }

        [Test]
        public async Task Handle_ActualizaDocumento_CambiaElDocumento()
        {
            // Arrange
            var documentoOriginal = new Documento(TipoDocumento.Cedula, "12345678");
            var personaExistente = new Persona("Juan", "Perez", documentoOriginal, TipoPersona.Medico);

            var command = new ActualizarPersonaCommand
            {
                Id = 1,
                Nombre = "Juan",
                Apellido = "Perez",
                TipoDocumento = TipoDocumento.Pasaporte,
                NumeroDocumento = "ABC12345"
            };

            Persona personaActualizada = null;
            _mockRepository
                .Setup(x => x.ObtenerPorId(1))
                .Returns(personaExistente);

            _mockRepository
                .Setup(x => x.Actualizar(It.IsAny<Persona>()))
                .Callback<Persona>(p => personaActualizada = p);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsNotNull(personaActualizada);
            Assert.AreEqual(TipoDocumento.Pasaporte, personaActualizada.Documento.TipoDocumento);
            Assert.AreEqual("ABC12345", personaActualizada.Documento.Numero);
        }

        [Test]
        public void Handle_ConNombreVacio_LanzaArgumentException()
        {
            // Arrange
            var documento = new Documento(TipoDocumento.Cedula, "12345678");
            var personaExistente = new Persona("Juan", "Perez", documento, TipoPersona.Medico);

            var command = new ActualizarPersonaCommand
            {
                Id = 1,
                Nombre = "",
                Apellido = "Perez",
                TipoDocumento = TipoDocumento.Cedula,
                NumeroDocumento = "12345678"
            };

            _mockRepository
                .Setup(x => x.ObtenerPorId(1))
                .Returns(personaExistente);

            // Act & Assert
            var ex = Assert.CatchAsync<ArgumentException>(async () =>
            {
                await _handler.Handle(command, CancellationToken.None);
            });

            Assert.IsNotNull(ex);
        }

        [Test]
        public void Handle_ConApellidoVacio_LanzaArgumentException()
        {
            // Arrange
            var documento = new Documento(TipoDocumento.Cedula, "12345678");
            var personaExistente = new Persona("Juan", "Perez", documento, TipoPersona.Medico);

            var command = new ActualizarPersonaCommand
            {
                Id = 1,
                Nombre = "Juan",
                Apellido = "",
                TipoDocumento = TipoDocumento.Cedula,
                NumeroDocumento = "12345678"
            };

            _mockRepository
                .Setup(x => x.ObtenerPorId(1))
                .Returns(personaExistente);

            // Act & Assert
            var ex = Assert.CatchAsync<ArgumentException>(async () =>
            {
                await _handler.Handle(command, CancellationToken.None);
            });

            Assert.IsNotNull(ex);
        }

        [Test]
        public async Task Handle_MantieneTipoPersonaOriginal()
        {
            // Arrange
            var documento = new Documento(TipoDocumento.Cedula, "12345678");
            var personaExistente = new Persona("Juan", "Perez", documento, TipoPersona.Medico);

            var command = new ActualizarPersonaCommand
            {
                Id = 1,
                Nombre = "Juan Actualizado",
                Apellido = "Perez Actualizado",
                TipoDocumento = TipoDocumento.Cedula,
                NumeroDocumento = "87654321"
            };

            Persona personaActualizada = null;
            _mockRepository
                .Setup(x => x.ObtenerPorId(1))
                .Returns(personaExistente);

            _mockRepository
                .Setup(x => x.Actualizar(It.IsAny<Persona>()))
                .Callback<Persona>(p => personaActualizada = p);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsNotNull(personaActualizada);
            Assert.AreEqual(TipoPersona.Medico, personaActualizada.TipoPersona);
        }
    }
}