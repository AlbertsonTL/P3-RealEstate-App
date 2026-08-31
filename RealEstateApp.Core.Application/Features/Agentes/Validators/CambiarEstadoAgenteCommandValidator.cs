using FluentValidation;
using RealEstateApp.Core.Application.Features.Agentes.Commands;

namespace RealEstateApp.Core.Application.Features.Agentes.Validators
{
    public class CambiarEstadoAgenteCommandValidator : AbstractValidator<CambiarEstadoAgenteCommand>
    {
        public CambiarEstadoAgenteCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("El Id del agente es requerido.")
                .NotNull().WithMessage("El Id no puede ser nulo.");

            RuleFor(x => x.Estado)
                .NotNull().WithMessage("El estado es requerido.");
        }
    }
}
