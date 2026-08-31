using FluentValidation;
using RealEstateApp.Core.Application.Features.TipoVentas.Commands;

namespace RealEstateApp.Core.Application.Features.TipoVentas.Validators
{
    public class ActualizarTipoVentaCommandValidator : AbstractValidator<ActualizarTipoVentaCommand>
    {
        public ActualizarTipoVentaCommandValidator()
        {
            RuleFor(v => v.Id)
                .NotEmpty().WithMessage("El ID es requerido.");

            RuleFor(v => v.Nombre)
                .NotEmpty().WithMessage("El nombre es requerido.")
                .MaximumLength(100).WithMessage("El nombre no debe exceder los 100 caracteres.");

            RuleFor(v => v.Descripcion)
                .NotEmpty().WithMessage("La descripción es requerida.")
                .MaximumLength(500).WithMessage("La descripción no debe exceder los 500 caracteres.");
        }
    }
}
