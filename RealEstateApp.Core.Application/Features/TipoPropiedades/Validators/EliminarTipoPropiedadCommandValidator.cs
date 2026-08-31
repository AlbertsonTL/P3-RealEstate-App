using FluentValidation;
using RealEstateApp.Core.Application.Features.TipoPropiedades.Commands;

namespace RealEstateApp.Core.Application.Features.TipoPropiedades.Validators
{
    public class EliminarTipoPropiedadCommandValidator : AbstractValidator<EliminarTipoPropiedadCommand>
    {
        public EliminarTipoPropiedadCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("El Id es requerido.")
                .GreaterThan(0).WithMessage("El Id debe ser mayor a 0.");
        }
    }
}
