using AutoMapper;
using Moq;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.Features.TipoPropiedades.Commands;
using RealEstateApp.Core.Application.Features.TipoPropiedades.Handlers;
using RealEstateApp.Core.Domain.Entidades;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;
using Xunit;

namespace RealEstateApp.Tests.Unitarias.Handlers
{
    public class CrearTipoPropiedadCommandHandlerTests
    {
        private readonly Mock<IRepositorioGenerico<TipoPropiedad>> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CrearTipoPropiedadCommandHandler _handler;

        public CrearTipoPropiedadCommandHandlerTests()
        {
            _repoMock    = new Mock<IRepositorioGenerico<TipoPropiedad>>();
            _mapperMock  = new Mock<IMapper>();
            _handler     = new CrearTipoPropiedadCommandHandler(_repoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_DebeRetornarTipoPropiedadDto_CuandoSeCreaExitosamente()
        {
            var request  = new CrearTipoPropiedadCommand { Nombre = "Apartamento", Descripcion = "Unidad residencial" };
            var entidad  = new TipoPropiedad { Id = 1, Nombre = "Apartamento", Descripcion = "Unidad residencial" };
            var dto      = new TipoPropiedadDto { Id = 1, Nombre = "Apartamento", Descripcion = "Unidad residencial" };

            _mapperMock.Setup(m => m.Map<TipoPropiedad>(request)).Returns(new TipoPropiedad { Nombre = "Apartamento", Descripcion = "Unidad residencial" });
            _repoMock.Setup(r => r.AgregarAsync(It.IsAny<TipoPropiedad>())).ReturnsAsync(entidad);
            _mapperMock.Setup(m => m.Map<TipoPropiedadDto>(entidad)).Returns(dto);

            var resultado = await _handler.Handle(request, CancellationToken.None);

            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
            Assert.Equal("Apartamento", resultado.Nombre);
            _repoMock.Verify(r => r.AgregarAsync(It.IsAny<TipoPropiedad>()), Times.Once);
        }

        [Fact]
        public async Task Handle_DebeMapearComando_AntesDeAgregar()
        {
            var request = new CrearTipoPropiedadCommand { Nombre = "Casa", Descripcion = "Vivienda unifamiliar" };
            var entidad = new TipoPropiedad { Id = 2, Nombre = "Casa", Descripcion = "Vivienda unifamiliar" };
            var dto     = new TipoPropiedadDto { Id = 2, Nombre = "Casa" };

            _mapperMock.Setup(m => m.Map<TipoPropiedad>(request)).Returns(new TipoPropiedad { Nombre = "Casa" });
            _repoMock.Setup(r => r.AgregarAsync(It.IsAny<TipoPropiedad>())).ReturnsAsync(entidad);
            _mapperMock.Setup(m => m.Map<TipoPropiedadDto>(entidad)).Returns(dto);

            await _handler.Handle(request, CancellationToken.None);

            _mapperMock.Verify(m => m.Map<TipoPropiedad>(request), Times.Once);
        }

        [Fact]
        public async Task Handle_DebeRetornarDto_ConIdAsignado()
        {
            var request = new CrearTipoPropiedadCommand { Nombre = "Villa", Descripcion = "Propiedad de lujo" };
            var entidad = new TipoPropiedad { Id = 99, Nombre = "Villa", Descripcion = "Propiedad de lujo" };
            var dto     = new TipoPropiedadDto { Id = 99, Nombre = "Villa" };

            _mapperMock.Setup(m => m.Map<TipoPropiedad>(request)).Returns(new TipoPropiedad());
            _repoMock.Setup(r => r.AgregarAsync(It.IsAny<TipoPropiedad>())).ReturnsAsync(entidad);
            _mapperMock.Setup(m => m.Map<TipoPropiedadDto>(entidad)).Returns(dto);

            var resultado = await _handler.Handle(request, CancellationToken.None);

            Assert.Equal(99, resultado.Id);
        }
    }
}
