using MediatR;

namespace Personas.Application.Commands
{
    /// <summary>
    /// Comando para eliminar una persona
    /// </summary>
    public class EliminarPersonaCommand : IRequest<Unit>
    {
        public int Id { get; set; }

        public EliminarPersonaCommand(int id)
        {
            Id = id;
        }
    }
}
