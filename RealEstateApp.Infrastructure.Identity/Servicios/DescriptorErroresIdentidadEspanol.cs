using Microsoft.AspNetCore.Identity;

namespace RealEstateApp.Infrastructure.Identity.Servicios
{
    /// <summary>
    /// Traduce al español todos los mensajes de error que genera ASP.NET Core Identity
    /// (creación de usuarios, contraseñas, roles, etc.), ya que por defecto se generan en inglés.
    /// Se registra mediante AddErrorDescriber&lt;DescriptorErroresIdentidadEspanol&gt;() para que
    /// TODO el sistema (WebApp y WebApi) reciba mensajes en español sin necesidad de traducirlos
    /// manualmente en cada controlador o servicio.
    /// </summary>
    public class DescriptorErroresIdentidadEspanol : IdentityErrorDescriber
    {
        public override IdentityError DefaultError() => new()
        {
            Code = nameof(DefaultError),
            Description = "Ocurrió un error desconocido. Inténtalo nuevamente."
        };

        public override IdentityError ConcurrencyFailure() => new()
        {
            Code = nameof(ConcurrencyFailure),
            Description = "Los datos fueron modificados por otro proceso. Vuelve a intentarlo."
        };

        public override IdentityError PasswordMismatch() => new()
        {
            Code = nameof(PasswordMismatch),
            Description = "La contraseña actual no es correcta."
        };

        public override IdentityError InvalidToken() => new()
        {
            Code = nameof(InvalidToken),
            Description = "El token proporcionado no es válido o ha expirado."
        };

        public override IdentityError RecoveryCodeRedemptionFailed() => new()
        {
            Code = nameof(RecoveryCodeRedemptionFailed),
            Description = "No fue posible canjear el código de recuperación."
        };

        public override IdentityError LoginAlreadyAssociated() => new()
        {
            Code = nameof(LoginAlreadyAssociated),
            Description = "Ya existe un usuario con esta información de inicio de sesión."
        };

        public override IdentityError InvalidUserName(string? userName) => new()
        {
            Code = nameof(InvalidUserName),
            Description = $"El nombre de usuario '{userName}' no es válido; solo se permiten letras, números y los caracteres . _ @ +."
        };

        public override IdentityError InvalidEmail(string? email) => new()
        {
            Code = nameof(InvalidEmail),
            Description = $"El correo electrónico '{email}' no es válido."
        };

        public override IdentityError DuplicateUserName(string userName) => new()
        {
            Code = nameof(DuplicateUserName),
            Description = "El nombre de usuario ya está en uso. Por favor elige otro."
        };

        public override IdentityError DuplicateEmail(string email) => new()
        {
            Code = nameof(DuplicateEmail),
            Description = "Este correo electrónico ya está registrado."
        };

        public override IdentityError InvalidRoleName(string? role) => new()
        {
            Code = nameof(InvalidRoleName),
            Description = $"El nombre de rol '{role}' no es válido."
        };

        public override IdentityError DuplicateRoleName(string role) => new()
        {
            Code = nameof(DuplicateRoleName),
            Description = $"El rol '{role}' ya existe."
        };

        public override IdentityError UserAlreadyHasPassword() => new()
        {
            Code = nameof(UserAlreadyHasPassword),
            Description = "El usuario ya tiene una contraseña establecida."
        };

        public override IdentityError UserLockoutNotEnabled() => new()
        {
            Code = nameof(UserLockoutNotEnabled),
            Description = "El bloqueo de cuenta no está habilitado para este usuario."
        };

        public override IdentityError UserAlreadyInRole(string role) => new()
        {
            Code = nameof(UserAlreadyInRole),
            Description = $"El usuario ya pertenece al rol '{role}'."
        };

        public override IdentityError UserNotInRole(string role) => new()
        {
            Code = nameof(UserNotInRole),
            Description = $"El usuario no pertenece al rol '{role}'."
        };

        public override IdentityError PasswordTooShort(int length) => new()
        {
            Code = nameof(PasswordTooShort),
            Description = $"La contraseña debe tener al menos {length} caracteres."
        };

        public override IdentityError PasswordRequiresNonAlphanumeric() => new()
        {
            Code = nameof(PasswordRequiresNonAlphanumeric),
            Description = "La contraseña debe contener al menos un carácter especial (por ejemplo: ! @ # $ % &)."
        };

        public override IdentityError PasswordRequiresDigit() => new()
        {
            Code = nameof(PasswordRequiresDigit),
            Description = "La contraseña debe contener al menos un número (0-9)."
        };

        public override IdentityError PasswordRequiresLower() => new()
        {
            Code = nameof(PasswordRequiresLower),
            Description = "La contraseña debe contener al menos una letra minúscula (a-z)."
        };

        public override IdentityError PasswordRequiresUpper() => new()
        {
            Code = nameof(PasswordRequiresUpper),
            Description = "La contraseña debe contener al menos una letra mayúscula (A-Z)."
        };

        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars) => new()
        {
            Code = nameof(PasswordRequiresUniqueChars),
            Description = $"La contraseña debe contener al menos {uniqueChars} caracteres distintos."
        };
    }
}
