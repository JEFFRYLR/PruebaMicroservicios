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
    public class CrearPersonaCommandHandlerTests
    {
        private Mock<IPersonaRepository> _mockRepository;
        private CrearPersonaCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IPersonaRepository>();
            _handler = new CrearPersonaCommandHandler(_mockRepository.Object);
        }

        [Test]
        public async Task Handle_ConDatosValidos_DeberiaCrearPersona()
        {
            // Arrange
            var command = new CrearPersonaCommand
            {
                Nombre = "Juan",
                Apellido = "Perez",
                TipoDocumento = TipoDocumento.Cedula,
                NumeroDocumento = "12345678",
                TipoPersona = TipoPersona.Medico
            };

            Persona personaCapturada = null;
            _mockRepository
                .Setup(x => x.Crear(It.IsAny<Persona>()))
                .Callback<Persona>(p => personaCapturada = p);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            resultado.Should().BeGreaterOrEqualTo(0);
            _mockRepository.Verify(x => x.Crear(It.IsAny<Persona>()), Times.Once);
            personaCapturada.Should().NotBeNull();
            personaCapturada.Nombre.Should().Be("Juan");
            personaCapturada.Apellido.Should().Be("Perez");
            personaCapturada.TipoPersona.Should().Be(TipoPersona.Medico);
        }

        [Test]
        public async Task Handle_ConTipoPaciente_DeberiaCrearPaciente()
        {
            // Arrange
            var command = new CrearPersonaCommand
            {
                Nombre = "Maria",
                Apellido = "Gonzalez",
                TipoDocumento = TipoDocumento.Pasaporte,
                NumeroDocumento = "ABC123",
                TipoPersona = TipoPersona.Paciente
            };

            Persona personaCapturada = null;
            _mockRepository
                .Setup(x => x.Crear(It.IsAny<Persona>()))
                .Callback<Persona>(p => personaCapturada = p);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            resultado.Should().BeGreaterOrEqualTo(0);
            _mockRepository.Verify(x => x.Crear(It.IsAny<Persona>()), Times.Once);
            personaCapturada.Should().NotBeNull();
            personaCapturada.TipoPersona.Should().Be(TipoPersona.Paciente);
        }

        [Test]
        public void Handle_ConNombreVacio_DeberiaLanzarException()
        {
            // Arrange
            var command = new CrearPersonaCommand
            {
                Nombre = "",
                Apellido = "Perez",
                TipoDocumento = TipoDocumento.Cedula,
                NumeroDocumento = "12345678",
                TipoPersona = TipoPersona.Medico
            };

            // Act
            Func<Task> action = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            action.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*nombre*");
        }

        [Test]
        public void Handle_ConApellidoVacio_DeberiaLanzarException()
        {
            // Arrange
            var command = new CrearPersonaCommand
            {
                Nombre = "Juan",
                Apellido = "",
                TipoDocumento = TipoDocumento.Cedula,
                NumeroDocumento = "12345678",
                TipoPersona = TipoPersona.Medico
            };

            // Act
            Func<Task> action = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            action.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*apellido*");
        }

        [Test]
        public void Handle_ConDocumentoVacio_DeberiaLanzarException()
        {
            // Arrange
            var command = new CrearPersonaCommand
            {
                Nombre = "Juan",
                Apellido = "Perez",
                TipoDocumento = TipoDocumento.Cedula,
                NumeroDocumento = "",
                TipoPersona = TipoPersona.Medico
            };

            // Act
            Func<Task> action = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            action.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*numero*");
        }

        [Test]
        public async Task Handle_VerificaQueLlamaCrearUnaVez()
        {
            // Arrange
            var command = new CrearPersonaCommand
            {
                Nombre = "Carlos",
                Apellido = "Lopez",
                TipoDocumento = TipoDocumento.Cedula,
                NumeroDocumento = "87654321",
                TipoPersona = TipoPersona.Medico
            };

            _mockRepository
                .Setup(x => x.Crear(It.IsAny<Persona>()));

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockRepository.Verify(
                x => x.Crear(It.Is<Persona>(p =>
                    p.Nombre == "Carlos" &&
                    p.Apellido == "Lopez"
                )),
                Times.Once
            );
        }

        [Test]
        public async Task Handle_ConDiferentesTiposDocumento_DeberiaCrearCorrectamente()
        {
            // Arrange
            var command = new CrearPersonaCommand
            {
                Nombre = "John",
                Apellido = "Smith",
                TipoDocumento = TipoDocumento.Pasaporte,
                NumeroDocumento = "XYZ789",
                TipoPersona = TipoPersona.Medico
            };

            Persona personaCapturada = null;
            _mockRepository
                .Setup(x => x.Crear(It.IsAny<Persona>()))
                .Callback<Persona>(p => personaCapturada = p);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            personaCapturada.Should().NotBeNull();
            personaCapturada.Documento.Should().NotBeNull();
            personaCapturada.Documento.TipoDocumento.Should().Be(TipoDocumento.Pasaporte);
            personaCapturada.Documento.Numero.Should().Be("XYZ789");
        }

        [Test]
        public async Task Handle_ConDocumentoCedula_DeberiaCrearConDocumentoCorrecto()
        {
            // Arrange
            var command = new CrearPersonaCommand
            {
                Nombre = "Ana",
                Apellido = "Martinez",
                TipoDocumento = TipoDocumento.Cedula,
                NumeroDocumento = "11223344",
                TipoPersona = TipoPersona.Paciente
            };

            Persona personaCapturada = null;
            _mockRepository
                .Setup(x => x.Crear(It.IsAny<Persona>()))
                .Callback<Persona>(p => personaCapturada = p);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            personaCapturada.Should().NotBeNull();
            personaCapturada.Documento.TipoDocumento.Should().Be(TipoDocumento.Cedula);
            personaCapturada.Documento.Numero.Should().Be("11223344");
        }
    }
}