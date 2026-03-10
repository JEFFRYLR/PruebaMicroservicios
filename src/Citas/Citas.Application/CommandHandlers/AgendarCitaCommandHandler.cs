using Citas.Application.Commands;
using Citas.Domain.Entities;
using Citas.Domain.Enums;
using Citas.Domain.Interfaces;
using MediatR;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Citas.Application.CommandHandlers
{
    /// <summary>
    /// Handler para agendar una nueva cita con validación de personas
    /// </summary>
    public class AgendarCitaCommandHandler : IRequestHandler<AgendarCitaCommand, int>
    {
        private readonly ICitaRepository _repository;
        private readonly IPersonasExternoService _personasService;

        public AgendarCitaCommandHandler(
            ICitaRepository repository, 
            IPersonasExternoService personasService)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _personasService = personasService ?? throw new ArgumentNullException(nameof(personasService));
        }

        public async Task<int> Handle(AgendarCitaCommand request, CancellationToken cancellationToken)
        {
            //  VALIDACIÓN 1: Verificar que el médico exista
            Debug.WriteLine($"Validando médico con ID: {request.MedicoId}");
            
            bool medicoExiste = await _personasService.ExisteMedicoAsync(request.MedicoId);
            
            if (!medicoExiste)
            {
                throw new InvalidOperationException(
                    $"No se puede agendar la cita. El médico con ID {request.MedicoId} no existe o no está activo en el sistema.");
            }

            Debug.WriteLine($" Médico {request.MedicoId} validado correctamente");

            //  VALIDACIÓN 2: Verificar que el paciente exista
            Debug.WriteLine($"Validando paciente con ID: {request.PacienteId}");
            
            bool pacienteExiste = await _personasService.ExistePacienteAsync(request.PacienteId);
            
            if (!pacienteExiste)
            {
                throw new InvalidOperationException(
                    $"No se puede agendar la cita. El paciente con ID {request.PacienteId} no existe o no está activo en el sistema.");
            }

            Debug.WriteLine($" Paciente {request.PacienteId} validado correctamente");

            //  Si ambas validaciones pasaron, crear la cita
            var cita = new Cita(
                request.FechaCita,
                request.Lugar,
                request.MedicoId,
                request.PacienteId,
                request.Motivo
            );

            _repository.Crear(cita);
            _repository.GuardarCambios();

            Debug.WriteLine($" Cita {cita.Id} creada exitosamente");

            return await Task.FromResult(cita.Id);
        }
    }
}
