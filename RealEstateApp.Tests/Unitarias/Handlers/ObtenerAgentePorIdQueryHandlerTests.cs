using Moq;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.Features.Agentes.Handlers;
using RealEstateApp.Core.Application.Features.Agentes.Queries;
using RealEstateApp.Core.Application.Interfaces;
using Xunit;

namespace RealEstateApp.Tests.Unitarias.Handlers
{
    public class ObtenerAgentePorIdQueryHandlerTests
    {
        private readonly Mock<IServicioCuenta> _servicioCuentaMock;
        private readonly ObtenerAgentePorIdQueryHandler _handler;

        public ObtenerAgentePorIdQueryHandlerTests()
        {
            _servicioCuentaMock = new Mock<IServicioCuenta>();
            _handler            = new ObtenerAgentePorIdQueryHandler(_servicioCuentaMock.Object);
        }

        [Fact]
        public async Task Handle_DebeRetornarAgenteDto_CuandoAgenteExiste()
        {
            var agenteDto = new AgenteDto { Id = "abc-123", Nombre = "Carlos", Apellido = "Romero" };
            _servicioCuentaMock.Setup(s => s.ObtenerAgentePorIdAsync("abc-123")).ReturnsAsync(agenteDto);

            var resultado = await _handler.Handle(new ObtenerAgentePorIdQuery { Id = "abc-123" }, CancellationToken.None);

            Assert.NotNull(resultado);
            Assert.Equal("abc-123", resultado.Id);
            Assert.Equal("Carlos",  resultado.Nombre);
        }

        [Fact]
        public async Task Handle_DebeRetornarNull_CuandoAgenteNoExiste()
        {
            _servicioCuentaMock.Setup(s => s.ObtenerAgentePorIdAsync("no-existe")).ReturnsAsync((AgenteDto?)null);

            var resultado = await _handler.Handle(new ObtenerAgentePorIdQuery { Id = "no-existe" }, CancellationToken.None);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task Handle_DebeLlamarObtenerAgentePorIdAsync_UnaVez()
        {
            _servicioCuentaMock.Setup(s => s.ObtenerAgentePorIdAsync(It.IsAny<string>())).ReturnsAsync((AgenteDto?)null);

            await _handler.Handle(new ObtenerAgentePorIdQuery { Id = "cualquier-id" }, CancellationToken.None);

            _servicioCuentaMock.Verify(s => s.ObtenerAgentePorIdAsync("cualquier-id"), Times.Once);
        }

        [Fact]
        public async Task Handle_DebeRetornarAgenteConTodosLosCampos_CuandoExiste()
        {
            var agenteDto = new AgenteDto
            {
                Id                  = "id-001",
                Nombre              = "Ana",
                Apellido            = "García",
                Correo              = "ana@test.com",
                Telefono            = "809-555-0001",
                CantidadPropiedades = 5
            };
            _servicioCuentaMock.Setup(s => s.ObtenerAgentePorIdAsync("id-001")).ReturnsAsync(agenteDto);

            var resultado = await _handler.Handle(new ObtenerAgentePorIdQuery { Id = "id-001" }, CancellationToken.None);

            Assert.Equal("Ana",          resultado!.Nombre);
            Assert.Equal("García",       resultado.Apellido);
            Assert.Equal("ana@test.com", resultado.Correo);
            Assert.Equal(5,              resultado.CantidadPropiedades);
        }
    }
}
