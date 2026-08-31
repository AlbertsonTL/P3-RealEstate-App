using FluentValidation;
using RealEstateApp.Core.Application.Features.TipoVentas.Commands;

namespace RealEstateApp.Core.Application.Features.TipoVentas.Validators
{
    public class EliminarTipoVentaCommandValidator : AbstractValidator<EliminarTipoVentaCommand>
    {
        public EliminarTipoVentaCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("El Id es requerido.")
                .GreaterThan(0).WithMessage("El Id debe ser mayor a 0.");
        }
    }
}
