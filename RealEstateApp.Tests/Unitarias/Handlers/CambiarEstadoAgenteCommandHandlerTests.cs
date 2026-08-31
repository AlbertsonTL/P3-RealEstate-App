using Moq;
using RealEstateApp.Core.Application.Features.Agentes.Commands;
using RealEstateApp.Core.Application.Features.Agentes.Handlers;
using RealEstateApp.Core.Application.Interfaces;
using Xunit;

namespace RealEstateApp.Tests.Unitarias.Handlers
{
    public class CambiarEstadoAgenteCommandHandlerTests
    {
        private readonly Mock<IServicioCuenta> _servicioCuentaMock;
        private readonly CambiarEstadoAgenteCommandHandler _handler;

        public CambiarEstadoAgenteCommandHandlerTests()
        {
            _servicioCuentaMock = new Mock<IServicioCuenta>();
            _handler = new CambiarEstadoAgenteCommandHandler(_servicioCuentaMock.Object);
        }

        [Fact]
        public async Task Handle_DebeRetornarTrue_CuandoCambioDeEstadoExitoso()
        {
            _servicioCuentaMock.Setup(s => s.CambiarEstadoUsuarioAsync("agente-01", true)).ReturnsAsync(true);

            var resultado = await _handler.Handle(
                new CambiarEstadoAgenteCommand { Id = "agente-01", Estado = true }, CancellationToken.None);

            Assert.True(resultado);
            _servicioCuentaMock.Verify(s => s.CambiarEstadoUsuarioAsync("agente-01", true), Times.Once);
        }

        [Fact]
        public async Task Handle_DebeRetornarFalse_CuandoAgenteNoExiste()
        {
            _servicioCuentaMock.Setup(s => s.CambiarEstadoUsuarioAsync("no-existe", false)).ReturnsAsync(false);

            var resultado = await _handler.Handle(
                new CambiarEstadoAgenteCommand { Id = "no-existe", Estado = false }, CancellationToken.None);

            Assert.False(resultado);
        }

        [Fact]
        public async Task Handle_DebeLlamarActivarInactivarUsuario_UnaVez()
        {
            _servicioCuentaMock.Setup(s => s.CambiarEstadoUsuarioAsync(It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(true);

            await _handler.Handle(new CambiarEstadoAgenteCommand { Id = "cualquier-id", Estado = true }, CancellationToken.None);

            _servicioCuentaMock.Verify(s => s.CambiarEstadoUsuarioAsync("cualquier-id", true), Times.Once);
        }

        [Fact]
        public async Task Handle_NoDebeLlamarOtrosServicios_SoloActivarInactivar()
        {
            _servicioCuentaMock.Setup(s => s.CambiarEstadoUsuarioAsync(It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(true);

            await _handler.Handle(new CambiarEstadoAgenteCommand { Id = "agente-x", Estado = false }, CancellationToken.None);

            // Solo se llama ActivarInactivar, ningún otro método del servicio
            _servicioCuentaMock.Verify(s => s.ObtenerAgentesAsync(), Times.Never);
            _servicioCuentaMock.Verify(s => s.ObtenerAgentePorIdAsync(It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [InlineData("agente-a")]
        [InlineData("agente-b")]
        [InlineData("agente-c")]
        public async Task Handle_DebePasarIdCorrecto_EnCualquierCaso(string agenteId)
        {
            _servicioCuentaMock.Setup(s => s.CambiarEstadoUsuarioAsync(agenteId, true)).ReturnsAsync(true);

            await _handler.Handle(new CambiarEstadoAgenteCommand { Id = agenteId, Estado = true }, CancellationToken.None);

            _servicioCuentaMock.Verify(s => s.CambiarEstadoUsuarioAsync(agenteId, true), Times.Once);
        }
    }
}
