using FluentValidation;
using RealEstateApp.Core.Application.Features.Mejoras.Commands;

namespace RealEstateApp.Core.Application.Features.Mejoras.Validators
{
    public class EliminarMejoraCommandValidator : AbstractValidator<EliminarMejoraCommand>
    {
        public EliminarMejoraCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("El Id es requerido.")
                .GreaterThan(0).WithMessage("El Id debe ser mayor a 0.");
        }
    }
}
