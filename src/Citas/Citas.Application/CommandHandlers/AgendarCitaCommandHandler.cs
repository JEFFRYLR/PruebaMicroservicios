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
            // NUEVO: Configurar token JWT antes de hacer llamadas al servicio externo
            if (!string.IsNullOrEmpty(request.BearerToken))
            {
                _personasService.SetAuthorizationToken(request.BearerToken);
                Debug.WriteLine("Token JWT configurado en PersonasExternoService");
            }

            // VALIDACIÓN 1: Verificar que el médico exista
            Debug.WriteLine(string.Format("Validando médico con ID: {0}", request.MedicoId));
            
            bool medicoExiste = await _personasService.ExisteMedicoAsync(request.MedicoId);
            
            if (!medicoExiste)
            {
                throw new InvalidOperationException(
                    string.Format("No se puede agendar la cita. El médico con ID {0} no existe o no está activo en el sistema.", request.MedicoId));
            }

            Debug.WriteLine(string.Format(" Médico {0} validado correctamente", request.MedicoId));

            // VALIDACIÓN 2: Verificar que el paciente exista
            Debug.WriteLine(string.Format("Validando paciente con ID: {0}", request.PacienteId));
            
            bool pacienteExiste = await _personasService.ExistePacienteAsync(request.PacienteId);
            
            if (!pacienteExiste)
            {
                throw new InvalidOperationException(
                    string.Format("No se puede agendar la cita. El paciente con ID {0} no existe o no está activo en el sistema.", request.PacienteId));
            }

            Debug.WriteLine(string.Format(" Paciente {0} validado correctamente", request.PacienteId));

            // Si ambas validaciones pasaron, crear la cita
            var cita = new Cita(
                request.FechaCita,
                request.Lugar,
                request.MedicoId,
                request.PacienteId,
                request.Motivo
            );

            _repository.Crear(cita);
            _repository.GuardarCambios();

            Debug.WriteLine(string.Format(" Cita {0} creada exitosamente", cita.Id));

            return await Task.FromResult(cita.Id);
        }
    }
}
