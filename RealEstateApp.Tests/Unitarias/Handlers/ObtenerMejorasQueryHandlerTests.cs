using AutoMapper;
using Moq;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.Features.Mejoras.Handlers;
using RealEstateApp.Core.Application.Features.Mejoras.Queries;
using RealEstateApp.Core.Domain.Entidades;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;
using Xunit;

namespace RealEstateApp.Tests.Unitarias.Handlers
{
    public class ObtenerMejorasQueryHandlerTests
    {
        private readonly Mock<IRepositorioGenerico<Mejora>> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ObtenerMejorasQueryHandler _handler;

        public ObtenerMejorasQueryHandlerTests()
        {
            _repoMock   = new Mock<IRepositorioGenerico<Mejora>>();
            _mapperMock = new Mock<IMapper>();
            _handler    = new ObtenerMejorasQueryHandler(_repoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_DebeRetornarColeccion_CuandoExistenMejoras()
        {
            var entidades = new List<Mejora>
            {
                new() { Id = 1, Nombre = "Piscina",  Descripcion = "Piscina olímpica" },
                new() { Id = 2, Nombre = "Garaje",   Descripcion = "Garaje doble" }
            };
            var dtos = new List<MejoraDto>
            {
                new() { Id = 1, Nombre = "Piscina" },
                new() { Id = 2, Nombre = "Garaje" }
            };

            _repoMock.Setup(r => r.ObtenerTodosAsync()).ReturnsAsync(entidades);
            _mapperMock.Setup(m => m.Map<IEnumerable<MejoraDto>>(entidades)).Returns(dtos);

            var resultado = await _handler.Handle(new ObtenerMejorasQuery(), CancellationToken.None);

            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count());
        }

        [Fact]
        public async Task Handle_DebeRetornarColeccionVacia_CuandoNoHayMejoras()
        {
            var entidades = new List<Mejora>();
            var dtos      = new List<MejoraDto>();

            _repoMock.Setup(r => r.ObtenerTodosAsync()).ReturnsAsync(entidades);
            _mapperMock.Setup(m => m.Map<IEnumerable<MejoraDto>>(entidades)).Returns(dtos);

            var resultado = await _handler.Handle(new ObtenerMejorasQuery(), CancellationToken.None);

            Assert.Empty(resultado);
        }

        [Fact]
        public async Task Handle_DebeLlamarObtenerTodosAsync_UnaVez()
        {
            var entidades = new List<Mejora>();
            _repoMock.Setup(r => r.ObtenerTodosAsync()).ReturnsAsync(entidades);
            _mapperMock.Setup(m => m.Map<IEnumerable<MejoraDto>>(entidades)).Returns(new List<MejoraDto>());

            await _handler.Handle(new ObtenerMejorasQuery(), CancellationToken.None);

            _repoMock.Verify(r => r.ObtenerTodosAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_DebeMapearEntidadesADtos_UnaVez()
        {
            var entidades = new List<Mejora> { new() { Id = 1, Nombre = "Test" } };
            var dtos      = new List<MejoraDto> { new() { Id = 1, Nombre = "Test" } };

            _repoMock.Setup(r => r.ObtenerTodosAsync()).ReturnsAsync(entidades);
            _mapperMock.Setup(m => m.Map<IEnumerable<MejoraDto>>(entidades)).Returns(dtos);

            await _handler.Handle(new ObtenerMejorasQuery(), CancellationToken.None);

            _mapperMock.Verify(m => m.Map<IEnumerable<MejoraDto>>(entidades), Times.Once);
        }
    }
}
