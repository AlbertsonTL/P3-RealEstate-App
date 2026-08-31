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
    public class ObtenerPropiedadPorCodigoQueryHandlerTests
    {
        private readonly Mock<IRepositorioPropiedad> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ObtenerPropiedadPorCodigoQueryHandler _handler;

        public ObtenerPropiedadPorCodigoQueryHandlerTests()
        {
            _repoMock   = new Mock<IRepositorioPropiedad>();
            _mapperMock = new Mock<IMapper>();
            _handler    = new ObtenerPropiedadPorCodigoQueryHandler(_repoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_DebeRetornarPropiedad_CuandoExisteElCodigo()
        {
            string codigoBusqueda = "ABC123";
            var propiedadMock     = new Propiedad { Id = 1, Codigo = codigoBusqueda };
            var dtoMock           = new PropiedadDto { Id = 1, Codigo = codigoBusqueda };
            _repoMock.Setup(r => r.ObtenerPorCodigoAsync(codigoBusqueda)).ReturnsAsync(propiedadMock);
            _mapperMock.Setup(m => m.Map<PropiedadDto>(propiedadMock)).Returns(dtoMock);

            var resultado = await _handler.Handle(new ObtenerPropiedadPorCodigoQuery { Codigo = codigoBusqueda }, CancellationToken.None);

            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
            Assert.Equal(codigoBusqueda, resultado.Codigo);
        }

        [Fact]
        public async Task Handle_DebeRetornarNull_CuandoCodigoNoExiste()
        {
            string codigoBusqueda = "NOEXISTE";
            _repoMock.Setup(r => r.ObtenerPorCodigoAsync(codigoBusqueda)).ReturnsAsync((Propiedad?)null);
            _mapperMock.Setup(m => m.Map<PropiedadDto>(null)).Returns((PropiedadDto?)null!);

            var resultado = await _handler.Handle(new ObtenerPropiedadPorCodigoQuery { Codigo = codigoBusqueda }, CancellationToken.None);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task Handle_DebeLlamarObtenerPorCodigoAsync_UnaVez()
        {
            string codigo    = "XYZ999";
            var propiedad    = new Propiedad { Id = 7, Codigo = codigo };
            _repoMock.Setup(r => r.ObtenerPorCodigoAsync(codigo)).ReturnsAsync(propiedad);
            _mapperMock.Setup(m => m.Map<PropiedadDto>(propiedad)).Returns(new PropiedadDto { Id = 7, Codigo = codigo });

            await _handler.Handle(new ObtenerPropiedadPorCodigoQuery { Codigo = codigo }, CancellationToken.None);

            _repoMock.Verify(r => r.ObtenerPorCodigoAsync(codigo), Times.Once);
        }

        [Theory]
        [InlineData("ABC001")]
        [InlineData("DEF002")]
        [InlineData("GHI003")]
        public async Task Handle_DebeRetornarDto_ConCodigoCorrecto(string codigo)
        {
            var propiedad = new Propiedad { Id = 1, Codigo = codigo };
            var dto       = new PropiedadDto { Id = 1, Codigo = codigo };
            _repoMock.Setup(r => r.ObtenerPorCodigoAsync(codigo)).ReturnsAsync(propiedad);
            _mapperMock.Setup(m => m.Map<PropiedadDto>(propiedad)).Returns(dto);

            var resultado = await _handler.Handle(new ObtenerPropiedadPorCodigoQuery { Codigo = codigo }, CancellationToken.None);

            Assert.Equal(codigo, resultado.Codigo);
        }
    }
}
