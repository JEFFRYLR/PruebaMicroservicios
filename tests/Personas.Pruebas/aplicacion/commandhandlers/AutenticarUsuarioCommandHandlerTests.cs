using BCrypt.Net;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Personas.Application.CommandHandlers;
using Personas.Application.Commands;
using Personas.Application.Interfaces;
using Personas.Application.Services;
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
    public class AutenticarUsuarioCommandHandlerTests
    {
        private Mock<IUsuarioRepository> _mockUsuarioRepository;
        private Mock<ITokenService> _mockTokenService;
        private AutenticarUsuarioCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _mockUsuarioRepository = new Mock<IUsuarioRepository>();
            _mockTokenService = new Mock<ITokenService>();
            _handler = new AutenticarUsuarioCommandHandler(
                _mockUsuarioRepository.Object,
                _mockTokenService.Object
            );
        }

        [Test]
        public async Task Handle_ConCredencialesValidas_GeneraToken()
        {
            // Arrange
            var command = new AutenticarUsuarioCommand
            {
                NombreUsuario = "medico1",
                Password = "1234"
            };

            var passwordHash = BCrypt.Net.BCrypt.HashPassword("1234");
            var usuario = new Usuario(1, "medico1", passwordHash);
            
            var documento = new Documento(TipoDocumento.Cedula, "12345678");
            var persona = new Persona("Juan", "Pérez", documento, TipoPersona.Medico);
            
            _mockUsuarioRepository
                .Setup(x => x.ObtenerPorNombreUsuarioAsync("medico1"))
                .ReturnsAsync(usuario);

            _mockUsuarioRepository
                .Setup(x => x.ObtenerPersonaPorIdAsync(1))
                .ReturnsAsync(persona);

            _mockTokenService
                .Setup(x => x.GenerarToken(usuario, persona))
                .Returns("fake-jwt-token");

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual("fake-jwt-token", resultado.Token);
            Assert.AreEqual("medico1", resultado.NombreUsuario);
        }

        [Test]
        public async Task Handle_ConUsuarioInexistente_LanzaException()
        {
            // Arrange
            var command = new AutenticarUsuarioCommand
            {
                NombreUsuario = "noexiste",
                Password = "1234"
            };

            _mockUsuarioRepository
                .Setup(x => x.ObtenerPorNombreUsuarioAsync("noexiste"))
                .ReturnsAsync((Usuario)null);

            // Act
            UnauthorizedAccessException excepcion = null;
            try
            {
                await _handler.Handle(command, CancellationToken.None);
            }
            catch (UnauthorizedAccessException ex)
            {
                excepcion = ex;
            }

            // Assert
            Assert.IsNotNull(excepcion);
        }

        [Test]
        public async Task Handle_ConPasswordIncorrecto_LanzaException()
        {
            // Arrange
            var command = new AutenticarUsuarioCommand
            {
                NombreUsuario = "medico1",
                Password = "incorrecta"
            };

            var passwordHash = BCrypt.Net.BCrypt.HashPassword("1234");
            var usuario = new Usuario(1, "medico1", passwordHash);
            
            _mockUsuarioRepository
                .Setup(x => x.ObtenerPorNombreUsuarioAsync("medico1"))
                .ReturnsAsync(usuario);

            // Act
            UnauthorizedAccessException excepcion = null;
            try
            {
                await _handler.Handle(command, CancellationToken.None);
            }
            catch (UnauthorizedAccessException ex)
            {
                excepcion = ex;
            }

            // Assert
            Assert.IsNotNull(excepcion);
        }

        [Test]
        public async Task Handle_VerificaQueLlamaRepositoriosCorrectamente()
        {
            // Arrange
            var command = new AutenticarUsuarioCommand
            {
                NombreUsuario = "paciente1",
                Password = "1234"
            };

            var passwordHash = BCrypt.Net.BCrypt.HashPassword("1234");
            var usuario = new Usuario(2, "paciente1", passwordHash);
            
            var documento = new Documento(TipoDocumento.Pasaporte, "ABC123");
            var persona = new Persona("María", "González", documento, TipoPersona.Paciente);
            
            _mockUsuarioRepository
                .Setup(x => x.ObtenerPorNombreUsuarioAsync("paciente1"))
                .ReturnsAsync(usuario);

            _mockUsuarioRepository
                .Setup(x => x.ObtenerPersonaPorIdAsync(2))
                .ReturnsAsync(persona);

            _mockTokenService
                .Setup(x => x.GenerarToken(usuario, persona))
                .Returns("token-paciente");

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockUsuarioRepository.Verify(x => x.ObtenerPorNombreUsuarioAsync("paciente1"), Times.Once);
            _mockUsuarioRepository.Verify(x => x.ObtenerPersonaPorIdAsync(2), Times.Once);
            _mockTokenService.Verify(x => x.GenerarToken(usuario, persona), Times.Once);
        }

        [Test]
        public async Task Handle_RetornaDatosCorrectosDelUsuario()
        {
            // Arrange
            var command = new AutenticarUsuarioCommand
            {
                NombreUsuario = "medico1",
                Password = "1234"
            };

            var passwordHash = BCrypt.Net.BCrypt.HashPassword("1234");
            var usuario = new Usuario(1, "medico1", passwordHash);
            
            var documento = new Documento(TipoDocumento.Cedula, "12345678");
            var persona = new Persona("Juan", "Pérez", documento, TipoPersona.Medico);
            
            _mockUsuarioRepository
                .Setup(x => x.ObtenerPorNombreUsuarioAsync("medico1"))
                .ReturnsAsync(usuario);

            _mockUsuarioRepository
                .Setup(x => x.ObtenerPersonaPorIdAsync(1))
                .ReturnsAsync(persona);

            _mockTokenService
                .Setup(x => x.GenerarToken(usuario, persona))
                .Returns("jwt-token-123");

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.AreEqual(1, resultado.PersonaId);
            Assert.AreEqual("Juan", resultado.Nombre);
            Assert.AreEqual("Pérez", resultado.Apellido);
            Assert.AreEqual("Medico", resultado.TipoPersona);
        }
    }
}
