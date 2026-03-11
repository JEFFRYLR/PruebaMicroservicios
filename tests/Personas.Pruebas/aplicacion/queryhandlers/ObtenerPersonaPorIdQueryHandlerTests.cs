using FluentAssertions;
using Moq;
using NUnit.Framework;
using Personas.Application.DTOs;
using Personas.Application.Queries;
using Personas.Application.QueryHandlers;
using Personas.Domain.Entities;
using Personas.Domain.Enums;
using Personas.Domain.Interfaces;
using Personas.Domain.ValueObjects;
using System.Threading;
using System.Threading.Tasks;

namespace Personas.Pruebas.Aplicacion.QueryHandlers
{
    [TestFixture]
    public class ObtenerPersonaPorIdQueryHandlerTests
    {
        private Mock<IPersonaRepository> _mockRepository;
        private ObtenerPersonaPorIdQueryHandler _handler;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IPersonaRepository>();
            _handler = new ObtenerPersonaPorIdQueryHandler(_mockRepository.Object);
        }

        [Test]
        public async Task Handle_ConIdExistente_DeberiaRetornarPersonaDto()
        {
            // Arrange
            var documento = new Documento(TipoDocumento.Cedula, "12345678");
            var persona = new Persona("Juan", "Pérez", documento, TipoPersona.Medico);

            var query = new ObtenerPersonaPorIdQuery(1);
            
            _mockRepository
                .Setup(x => x.ObtenerPorId(1))
                .Returns(persona);

            // Act
            var resultado = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual("Juan", resultado.Nombre);
            Assert.AreEqual("Pérez", resultado.Apellido);
            Assert.AreEqual(TipoPersona.Medico, resultado.TipoPersona);
        }

        [Test]
        public async Task Handle_ConIdInexistente_DeberiaRetornarNull()
        {
            // Arrange
            var query = new ObtenerPersonaPorIdQuery(999);

            _mockRepository
                .Setup(x => x.ObtenerPorId(999))
                .Returns((Persona)null);

            // Act
            var resultado = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsNull(resultado);
        }

        [Test]
        public async Task Handle_ConPersonaPaciente_RetornaDtoConTipoCorrecto()
        {
            // Arrange
            var documento = new Documento(TipoDocumento.Pasaporte, "ABC123");
            var persona = new Persona("María", "González", documento, TipoPersona.Paciente);

            var query = new ObtenerPersonaPorIdQuery(2);

            _mockRepository
                .Setup(x => x.ObtenerPorId(2))
                .Returns(persona);

            // Act
            var resultado = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(TipoPersona.Paciente, resultado.TipoPersona);
        }

        [Test]
        public async Task Handle_VerificaQueLlamaRepositorioUnaVez()
        {
            // Arrange
            var documento = new Documento(TipoDocumento.Cedula, "12345678");
            var persona = new Persona("Carlos", "López", documento, TipoPersona.Medico);

            var query = new ObtenerPersonaPorIdQuery(5);

            _mockRepository
                .Setup(x => x.ObtenerPorId(5))
                .Returns(persona);

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _mockRepository.Verify(x => x.ObtenerPorId(5), Times.Once);
        }

        [Test]
        public async Task Handle_RetornaDtoConDocumentoCorrecto()
        {
            // Arrange
            var documento = new Documento(TipoDocumento.Pasaporte, "XYZ789");
            var persona = new Persona("Ana", "Martínez", documento, TipoPersona.Paciente);

            var query = new ObtenerPersonaPorIdQuery(3);

            _mockRepository
                .Setup(x => x.ObtenerPorId(3))
                .Returns(persona);

            // Act
            var resultado = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual("XYZ789", resultado.NumeroDocumento);
            Assert.AreEqual(TipoDocumento.Pasaporte, resultado.TipoDocumento);
        }
    }
}