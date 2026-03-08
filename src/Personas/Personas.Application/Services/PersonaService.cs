using Personas.Application.DTOs;
using Personas.Application.Interfaces;
using Personas.Domain.Entities;
using Personas.Domain.Enums;
using Personas.Domain.Interfaces;
using Personas.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;

namespace Personas.Application.Services
{
    public class PersonaService : IPersonaService
    {
        private readonly IPersonaRepository _personaRepository;

        public PersonaService(IPersonaRepository personaRepository)
        {
            _personaRepository = personaRepository;
        }

        public IEnumerable<PersonaDto> ObtenerTodas()
        {
            var personas = _personaRepository.ObtenerTodos();

            return personas.Select(p => new PersonaDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Apellido = p.Apellido,
                NumeroDocumento = p.Documento.Numero,
                TipoDocumento = p.Documento.TipoDocumento,
                TipoPersona = p.TipoPersona,
            });
        }

        public PersonaDto ObtenerPorId(int id)
        {
            var persona = _personaRepository.ObtenerPorId(id);

            if (persona == null)
                return null;

            return new PersonaDto
            {
                Id = persona.Id,
                Nombre = persona.Nombre,
                Apellido = persona.Apellido,
                NumeroDocumento = persona.Documento.Numero,
                TipoDocumento = persona.Documento.TipoDocumento,
                TipoPersona = persona.TipoPersona,
            };
        }

        public void Crear(PersonaDto dto)
        {
            var documento = new Documento(dto.TipoDocumento, dto.NumeroDocumento);

            var persona = new Persona(
                dto.Nombre,
                dto.Apellido,
                documento,
                dto.TipoPersona
            );

            _personaRepository.Crear(persona);
        }

        public void Actualizar(int id, PersonaDto dto)
        {
            var personaExistente = _personaRepository.ObtenerPorId(id);
            if (personaExistente == null)
                throw new System.Exception($"Persona con Id {id} no encontrada");

            var documento = new Documento(dto.TipoDocumento, dto.NumeroDocumento);

            personaExistente.Actualizar(
                dto.Nombre,
                dto.Apellido,
                documento,
                dto.TipoPersona
            );

            _personaRepository.Actualizar(personaExistente);
        }

        public void Eliminar(int id)
        {
            _personaRepository.Eliminar(id);
        }

        public IEnumerable<PersonaDto> ObtenerMedicos()
        {
            var personas = _personaRepository.ObtenerTodos();

            return personas
                .Where(p => p.TipoPersona == TipoPersona.Medico)
                .Select(p => new PersonaDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Apellido = p.Apellido,
                    NumeroDocumento = p.Documento.Numero,
                    TipoDocumento = p.Documento.TipoDocumento,
                    TipoPersona = p.TipoPersona,
                });
        }

        public IEnumerable<PersonaDto> ObtenerPacientes()
        {
            var personas = _personaRepository.ObtenerTodos();

            return personas
                .Where(p => p.TipoPersona == TipoPersona.Paciente)
                .Select(p => new PersonaDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Apellido = p.Apellido,
                    NumeroDocumento = p.Documento.Numero,
                    TipoDocumento = p.Documento.TipoDocumento,
                    TipoPersona = p.TipoPersona,
                });
        }
    }
}