using FluentValidation;
using RealEstateApp.Core.Application.Features.Mejoras.Commands;

namespace RealEstateApp.Core.Application.Features.Mejoras.Validators
{
    public class CrearMejoraCommandValidator : AbstractValidator<CrearMejoraCommand>
    {
        public CrearMejoraCommandValidator()
        {
            RuleFor(v => v.Nombre)
                .NotEmpty().WithMessage("El nombre es requerido.")
                .MaximumLength(100).WithMessage("El nombre no debe exceder los 100 caracteres.");

            RuleFor(v => v.Descripcion)
                .NotEmpty().WithMessage("La descripción es requerida.")
                .MaximumLength(500).WithMessage("La descripción no debe exceder los 500 caracteres.");
        }
    }
}
