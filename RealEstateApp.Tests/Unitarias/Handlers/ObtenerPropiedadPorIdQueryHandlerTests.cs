using AutoMapper;
using Moq;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.Features.Propiedades.Handlers;
using RealEstateApp.Core.Application.Features.Propiedades.Queries;
using RealEstateApp.Core.Domain.Entidades;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;
using Xunit;

namespace RealEstateApp.Tests.Unitarias.Handlers
{
    public class ObtenerPropiedadPorIdQueryHandlerTests
    {
        private readonly Mock<IRepositorioPropiedad> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ObtenerPropiedadPorIdQueryHandler _handler;

        public ObtenerPropiedadPorIdQueryHandlerTests()
        {
            _repoMock   = new Mock<IRepositorioPropiedad>();
            _mapperMock = new Mock<IMapper>();
            _handler    = new ObtenerPropiedadPorIdQueryHandler(_repoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_DebeRetornarPropiedad_CuandoExiste()
        {
            int idBusqueda      = 5;
            var propiedadMock   = new Propiedad { Id = idBusqueda, Codigo = "123456" };
            var dtoMock         = new PropiedadDto { Id = idBusqueda, Codigo = "123456" };
            _repoMock.Setup(r => r.ObtenerPorIdAsync(idBusqueda)).ReturnsAsync(propiedadMock);
            _mapperMock.Setup(m => m.Map<PropiedadDto>(propiedadMock)).Returns(dtoMock);

            var resultado = await _handler.Handle(new ObtenerPropiedadPorIdQuery { Id = idBusqueda }, CancellationToken.None);

            Assert.NotNull(resultado);
            Assert.Equal(idBusqueda, resultado.Id);
            Assert.Equal("123456", resultado.Codigo);
        }

        [Fact]
        public async Task Handle_DebeRetornarNull_CuandoNoExiste()
        {
            int idBusqueda = 999;
            _repoMock.Setup(r => r.ObtenerPorIdAsync(idBusqueda)).ReturnsAsync((Propiedad?)null);
            _mapperMock.Setup(m => m.Map<PropiedadDto>(null)).Returns((PropiedadDto?)null!);

            var resultado = await _handler.Handle(new ObtenerPropiedadPorIdQuery { Id = idBusqueda }, CancellationToken.None);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task Handle_DebeLlamarObtenerPorIdAsync_UnaVez()
        {
            int idBusqueda = 3;
            var propiedad  = new Propiedad { Id = idBusqueda, Codigo = "XYZ" };
            _repoMock.Setup(r => r.ObtenerPorIdAsync(idBusqueda)).ReturnsAsync(propiedad);
            _mapperMock.Setup(m => m.Map<PropiedadDto>(propiedad)).Returns(new PropiedadDto { Id = idBusqueda });

            await _handler.Handle(new ObtenerPropiedadPorIdQuery { Id = idBusqueda }, CancellationToken.None);

            _repoMock.Verify(r => r.ObtenerPorIdAsync(idBusqueda), Times.Once);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(50)]
        [InlineData(100)]
        public async Task Handle_DebeUsarIdCorrectamente_ConDistintosIds(int id)
        {
            var propiedad = new Propiedad { Id = id, Codigo = $"COD{id}" };
            var dto       = new PropiedadDto { Id = id, Codigo = $"COD{id}" };
            _repoMock.Setup(r => r.ObtenerPorIdAsync(id)).ReturnsAsync(propiedad);
            _mapperMock.Setup(m => m.Map<PropiedadDto>(propiedad)).Returns(dto);

            var resultado = await _handler.Handle(new ObtenerPropiedadPorIdQuery { Id = id }, CancellationToken.None);

            Assert.Equal(id, resultado.Id);
        }
    }
}
