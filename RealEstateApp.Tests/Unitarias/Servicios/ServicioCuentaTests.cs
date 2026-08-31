using Microsoft.AspNetCore.Identity;
using Moq;
using RealEstateApp.Core.Application.DTOs.Cuenta;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;
using RealEstateApp.Infrastructure.Identity.Entidades;
using RealEstateApp.Infrastructure.Identity.Servicios;
using Xunit;

namespace RealEstateApp.Tests.Unitarias.Servicios
{
    /// <summary>
    /// Pruebas unitarias para ServicioCuenta.
    /// Verifica la lógica de negocio de autenticación, registro y gestión de usuarios
    /// tal como exige el spec: "pruebas unitarias para servicios de lógica de negocio".
    /// </summary>
    public class ServicioCuentaLoginTests
    {
        private readonly Mock<UserManager<UsuarioAplicacion>> _userManagerMock;
        private readonly Mock<SignInManager<UsuarioAplicacion>> _signInManagerMock;
        private readonly Mock<ServicioJwt> _servicioJwtMock;
        private readonly Mock<IRepositorioPropiedad> _repoPropiedadMock;
        private readonly Mock<IServicioEmail> _servicioEmailMock;
        private readonly ServicioCuenta _servicio;

        public ServicioCuentaLoginTests()
        {
            // UserManager requiere IUserStore + opciones
            var storeMock = new Mock<IUserStore<UsuarioAplicacion>>();
            _userManagerMock = new Mock<UserManager<UsuarioAplicacion>>(
                storeMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            // SignInManager requiere UserManager + IHttpContextAccessor + IUserClaimsPrincipalFactory
            var contextAccessorMock = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            var claimsFactoryMock = new Mock<IUserClaimsPrincipalFactory<UsuarioAplicacion>>();
            _signInManagerMock = new Mock<SignInManager<UsuarioAplicacion>>(
                _userManagerMock.Object,
                contextAccessorMock.Object,
                claimsFactoryMock.Object,
                null!, null!, null!, null!);

            // ServicioJwt requires IOptions<JwtSettings>
            var jwtOptionsMock = new Mock<Microsoft.Extensions.Options.IOptions<
                RealEstateApp.Infrastructure.Identity.Configuracion.JwtSettings>>();
            jwtOptionsMock.Setup(o => o.Value).Returns(new RealEstateApp.Infrastructure.Identity.Configuracion.JwtSettings
            {
                Key = "TestKeyMuyLargaParaQueSeaValida12345",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                ExpirationMinutes = 60
            });
            _servicioJwtMock = new Mock<ServicioJwt>(jwtOptionsMock.Object);

            _repoPropiedadMock = new Mock<IRepositorioPropiedad>();
            _servicioEmailMock = new Mock<IServicioEmail>();

            _servicio = new ServicioCuenta(
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _servicioJwtMock.Object,
                _repoPropiedadMock.Object,
                _servicioEmailMock.Object);
        }

        // ── LoginAsync ────────────────────────────────────────────────────────

        [Fact]
        public async Task LoginAsync_DebeRetornarError_CuandoUsuarioNoExiste()
        {
            _userManagerMock.Setup(m => m.FindByNameAsync(It.IsAny<string>()))
                            .ReturnsAsync((UsuarioAplicacion?)null);
            _userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                            .ReturnsAsync((UsuarioAplicacion?)null);

            var resultado = await _servicio.LoginAsync(
                new SolicitudLoginDto { UsuarioOCorreo = "noexiste@test.com", Contrasena = "Pass123!" });

            Assert.True(resultado.TieneError);
            Assert.Equal("Credenciales incorrectas.", resultado.MensajeError);
        }

        [Fact]
        public async Task LoginAsync_DebeRetornarError_CuandoContrasenaIncorrecta()
        {
            var usuario = new UsuarioAplicacion
            {
                Id = "id-001",
                UserName = "test",
                Email = "test@test.com",
                EstaActivo = true,
                Nombre = "Test",
                Apellido = "User",
                Telefono = "000"
            };

            _userManagerMock.Setup(m => m.FindByNameAsync("test")).ReturnsAsync(usuario);
            _signInManagerMock.Setup(m => m.CheckPasswordSignInAsync(usuario, "WrongPass", false))
                              .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            var resultado = await _servicio.LoginAsync(
                new SolicitudLoginDto { UsuarioOCorreo = "test", Contrasena = "WrongPass" });

            Assert.True(resultado.TieneError);
            Assert.Equal("Credenciales incorrectas.", resultado.MensajeError);
        }

        [Fact]
        public async Task LoginAsync_DebeRetornarError_CuandoCuentaNoActivada()
        {
            var usuario = new UsuarioAplicacion
            {
                Id = "id-002",
                UserName = "inactivo",
                Email = "inactivo@test.com",
                EstaActivo = false,     // ← cuenta inactiva
                Nombre = "Juan",
                Apellido = "Pérez",
                Telefono = "000"
            };

            _userManagerMock.Setup(m => m.FindByNameAsync("inactivo")).ReturnsAsync(usuario);
            _signInManagerMock.Setup(m => m.CheckPasswordSignInAsync(usuario, "Pass123!", false))
                              .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            var resultado = await _servicio.LoginAsync(
                new SolicitudLoginDto { UsuarioOCorreo = "inactivo", Contrasena = "Pass123!" });

            Assert.True(resultado.TieneError);
            Assert.Contains("activada", resultado.MensajeError);
        }

        [Fact]
        public async Task LoginAsync_DebeRetornarTokenYRoles_CuandoCredencialesCorrectas()
        {
            var usuario = new UsuarioAplicacion
            {
                Id = "id-003",
                UserName = "adminuser",
                Email = "admin@test.com",
                EstaActivo = true,
                Nombre = "Admin",
                Apellido = "Test",
                Telefono = "000"
            };

            _userManagerMock.Setup(m => m.FindByNameAsync("adminuser")).ReturnsAsync(usuario);
            _signInManagerMock.Setup(m => m.CheckPasswordSignInAsync(usuario, "Admin123!", false))
                              .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
            _userManagerMock.Setup(m => m.GetRolesAsync(usuario))
                            .ReturnsAsync(new List<string> { "Administrador" });
            _servicioJwtMock.Setup(j => j.GenerarToken(usuario, It.IsAny<IList<string>>()))
                            .Returns("token-jwt-simulado");

            var resultado = await _servicio.LoginAsync(
                new SolicitudLoginDto { UsuarioOCorreo = "adminuser", Contrasena = "Admin123!" });

            Assert.False(resultado.TieneError);
            Assert.Equal("token-jwt-simulado", resultado.Token);
            Assert.Contains("Administrador", resultado.Roles);
            Assert.Equal("id-003", resultado.Id);
        }

        // ── CambiarEstadoUsuarioAsync ──────────────────────────────────────

        [Fact]
        public async Task CambiarEstadoUsuarioAsync_DebeRetornarFalse_CuandoUsuarioNoExiste()
        {
            _userManagerMock.Setup(m => m.FindByIdAsync("no-existe"))
                            .ReturnsAsync((UsuarioAplicacion?)null);

            var resultado = await _servicio.CambiarEstadoUsuarioAsync("no-existe", true);

            Assert.False(resultado);
        }

        [Fact]
        public async Task CambiarEstadoUsuarioAsync_DebeRetornarTrue_YCambiarEstado()
        {
            var usuario = new UsuarioAplicacion
            {
                Id = "id-004",
                EstaActivo = true,
                Nombre = "X",
                Apellido = "Y",
                Telefono = "000"
            };

            _userManagerMock.Setup(m => m.FindByIdAsync("id-004")).ReturnsAsync(usuario);
            _userManagerMock.Setup(m => m.UpdateAsync(It.IsAny<UsuarioAplicacion>()))
                            .ReturnsAsync(IdentityResult.Success);

            var resultado = await _servicio.CambiarEstadoUsuarioAsync("id-004", false);

            Assert.True(resultado);
            // El estado debe haberse invertido (true → false)
            Assert.False(usuario.EstaActivo);
        }

        // ── ObtenerAgentePorIdAsync ───────────────────────────────────────────

        [Fact]
        public async Task ObtenerAgentePorIdAsync_DebeRetornarNull_CuandoNoExiste()
        {
            _userManagerMock.Setup(m => m.FindByIdAsync("fantasma"))
                            .ReturnsAsync((UsuarioAplicacion?)null);

            var resultado = await _servicio.ObtenerAgentePorIdAsync("fantasma");

            Assert.Null(resultado);
        }

        [Fact]
        public async Task ObtenerAgentePorIdAsync_DebeRetornarAgenteDto_CuandoExiste()
        {
            var usuario = new UsuarioAplicacion
            {
                Id = "id-005",
                Nombre = "Carlos",
                Apellido = "López",
                Telefono = "000"
            };
            _userManagerMock.Setup(m => m.FindByIdAsync("id-005")).ReturnsAsync(usuario);

            var resultado = await _servicio.ObtenerAgentePorIdAsync("id-005");

            Assert.NotNull(resultado);
            Assert.Equal("id-005", resultado.Id);
            Assert.Equal("Carlos", resultado.Nombre);
        }
    }
}
