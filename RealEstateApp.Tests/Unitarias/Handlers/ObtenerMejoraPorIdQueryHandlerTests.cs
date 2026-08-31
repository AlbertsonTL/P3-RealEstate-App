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
    public class ObtenerMejoraPorIdQueryHandlerTests
    {
        private readonly Mock<IRepositorioGenerico<Mejora>> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ObtenerMejoraPorIdQueryHandler _handler;

        public ObtenerMejoraPorIdQueryHandlerTests()
        {
            _repoMock   = new Mock<IRepositorioGenerico<Mejora>>();
            _mapperMock = new Mock<IMapper>();
            _handler    = new ObtenerMejoraPorIdQueryHandler(_repoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_DebeRetornarMejoraDto_CuandoExiste()
        {
            var entidad = new Mejora { Id = 3, Nombre = "Jacuzzi", Descripcion = "Jacuzzi interior" };
            var dto     = new MejoraDto { Id = 3, Nombre = "Jacuzzi", Descripcion = "Jacuzzi interior" };

            _repoMock.Setup(r => r.ObtenerPorIdAsync(3)).ReturnsAsync(entidad);
            _mapperMock.Setup(m => m.Map<MejoraDto>(entidad)).Returns(dto);

            var resultado = await _handler.Handle(new ObtenerMejoraPorIdQuery { Id = 3 }, CancellationToken.None);

            Assert.NotNull(resultado);
            Assert.Equal(3, resultado.Id);
            Assert.Equal("Jacuzzi", resultado.Nombre);
        }

        [Fact]
        public async Task Handle_DebeRetornarNull_CuandoNoExiste()
        {
            _repoMock.Setup(r => r.ObtenerPorIdAsync(999)).ReturnsAsync((Mejora?)null);
            _mapperMock.Setup(m => m.Map<MejoraDto>(null!)).Returns((MejoraDto?)null!);

            var resultado = await _handler.Handle(new ObtenerMejoraPorIdQuery { Id = 999 }, CancellationToken.None);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task Handle_DebeLlamarObtenerPorIdAsync_UnaVez()
        {
            _repoMock.Setup(r => r.ObtenerPorIdAsync(5)).ReturnsAsync((Mejora?)null);
            _mapperMock.Setup(m => m.Map<MejoraDto>(null!)).Returns((MejoraDto?)null!);

            await _handler.Handle(new ObtenerMejoraPorIdQuery { Id = 5 }, CancellationToken.None);

            _repoMock.Verify(r => r.ObtenerPorIdAsync(5), Times.Once);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(50)]
        public async Task Handle_DebeUsarIdCorrectamente_ConDistintosIds(int id)
        {
            var entidad = new Mejora { Id = id, Nombre = $"Mejora{id}" };
            var dto     = new MejoraDto { Id = id, Nombre = $"Mejora{id}" };

            _repoMock.Setup(r => r.ObtenerPorIdAsync(id)).ReturnsAsync(entidad);
            _mapperMock.Setup(m => m.Map<MejoraDto>(entidad)).Returns(dto);

            var resultado = await _handler.Handle(new ObtenerMejoraPorIdQuery { Id = id }, CancellationToken.None);

            Assert.Equal(id, resultado.Id);
        }
    }
}
