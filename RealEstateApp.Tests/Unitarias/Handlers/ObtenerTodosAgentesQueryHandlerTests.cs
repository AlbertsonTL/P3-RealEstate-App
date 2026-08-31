using Moq;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.Features.Agentes.Handlers;
using RealEstateApp.Core.Application.Features.Agentes.Queries;
using RealEstateApp.Core.Application.Interfaces;
using Xunit;

namespace RealEstateApp.Tests.Unitarias.Handlers
{
    public class ObtenerTodosAgentesQueryHandlerTests
    {
        private readonly Mock<IServicioCuenta> _servicioCuentaMock;
        private readonly ObtenerTodosAgentesQueryHandler _handler;

        public ObtenerTodosAgentesQueryHandlerTests()
        {
            _servicioCuentaMock = new Mock<IServicioCuenta>();
            _handler            = new ObtenerTodosAgentesQueryHandler(_servicioCuentaMock.Object);
        }

        [Fact]
        public async Task Handle_DebeRetornarColeccionDeAgentes_CuandoExisten()
        {
            var agentesDto = new List<AgenteDto>
            {
                new AgenteDto { Id = "1", Nombre = "Juan", Apellido = "Perez" }
            };
            _servicioCuentaMock.Setup(s => s.ObtenerAgentesAsync()).ReturnsAsync(agentesDto);

            var resultado = await _handler.Handle(new ObtenerTodosAgentesQuery(), CancellationToken.None);

            Assert.NotNull(resultado);
            Assert.Single(resultado);
            Assert.Equal("Juan", resultado.First().Nombre);
        }

        [Fact]
        public async Task Handle_DebeRetornarColeccionVacia_CuandoNoHayAgentes()
        {
            _servicioCuentaMock.Setup(s => s.ObtenerAgentesAsync()).ReturnsAsync(new List<AgenteDto>());

            var resultado = await _handler.Handle(new ObtenerTodosAgentesQuery(), CancellationToken.None);

            Assert.Empty(resultado);
        }

        [Fact]
        public async Task Handle_DebeLlamarObtenerAgentesAsync_UnaVez()
        {
            _servicioCuentaMock.Setup(s => s.ObtenerAgentesAsync()).ReturnsAsync(new List<AgenteDto>());

            await _handler.Handle(new ObtenerTodosAgentesQuery(), CancellationToken.None);

            _servicioCuentaMock.Verify(s => s.ObtenerAgentesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_DebeRetornarTodosLosAgentes_CuandoHayVarios()
        {
            var agentes = new List<AgenteDto>
            {
                new AgenteDto { Id = "1", Nombre = "Ana",   Apellido = "García" },
                new AgenteDto { Id = "2", Nombre = "Luis",  Apellido = "López" },
                new AgenteDto { Id = "3", Nombre = "María", Apellido = "Martínez" }
            };
            _servicioCuentaMock.Setup(s => s.ObtenerAgentesAsync()).ReturnsAsync(agentes);

            var resultado = await _handler.Handle(new ObtenerTodosAgentesQuery(), CancellationToken.None);

            Assert.Equal(3, resultado.Count());
        }
    }
}
