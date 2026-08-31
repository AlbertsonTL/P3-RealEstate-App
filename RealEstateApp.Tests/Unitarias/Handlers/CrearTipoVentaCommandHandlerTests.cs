using AutoMapper;
using Moq;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.Features.TipoVentas.Commands;
using RealEstateApp.Core.Application.Features.TipoVentas.Handlers;
using RealEstateApp.Core.Domain.Entidades;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;
using Xunit;

namespace RealEstateApp.Tests.Unitarias.Handlers
{
    public class CrearTipoVentaCommandHandlerTests
    {
        private readonly Mock<IRepositorioGenerico<TipoVenta>> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CrearTipoVentaCommandHandler _handler;

        public CrearTipoVentaCommandHandlerTests()
        {
            _repoMock   = new Mock<IRepositorioGenerico<TipoVenta>>();
            _mapperMock = new Mock<IMapper>();
            _handler    = new CrearTipoVentaCommandHandler(_repoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_DebeRetornarTipoVentaDto_CuandoSeCreaExitosamente()
        {
            var request = new CrearTipoVentaCommand { Nombre = "Alquiler", Descripcion = "Modalidad de arrendamiento" };
            var entidad = new TipoVenta { Id = 1, Nombre = "Alquiler", Descripcion = "Modalidad de arrendamiento" };
            var dto     = new TipoVentaDto { Id = 1, Nombre = "Alquiler" };

            _mapperMock.Setup(m => m.Map<TipoVenta>(request)).Returns(new TipoVenta { Nombre = "Alquiler" });
            _repoMock.Setup(r => r.AgregarAsync(It.IsAny<TipoVenta>())).ReturnsAsync(entidad);
            _mapperMock.Setup(m => m.Map<TipoVentaDto>(entidad)).Returns(dto);

            var resultado = await _handler.Handle(request, CancellationToken.None);

            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
            Assert.Equal("Alquiler", resultado.Nombre);
            _repoMock.Verify(r => r.AgregarAsync(It.IsAny<TipoVenta>()), Times.Once);
        }

        [Fact]
        public async Task Handle_DebeMapearComando_AntesDeAgregar()
        {
            var request = new CrearTipoVentaCommand { Nombre = "Venta", Descripcion = "Compraventa directa" };
            var entidad = new TipoVenta { Id = 2, Nombre = "Venta" };
            var dto     = new TipoVentaDto { Id = 2, Nombre = "Venta" };

            _mapperMock.Setup(m => m.Map<TipoVenta>(request)).Returns(new TipoVenta { Nombre = "Venta" });
            _repoMock.Setup(r => r.AgregarAsync(It.IsAny<TipoVenta>())).ReturnsAsync(entidad);
            _mapperMock.Setup(m => m.Map<TipoVentaDto>(entidad)).Returns(dto);

            await _handler.Handle(request, CancellationToken.None);

            _mapperMock.Verify(m => m.Map<TipoVenta>(request), Times.Once);
        }

        [Fact]
        public async Task Handle_DebeRetornarDto_ConIdGeneradoPorRepositorio()
        {
            var request = new CrearTipoVentaCommand { Nombre = "Leasing", Descripcion = "Arrendamiento financiero" };
            var entidad = new TipoVenta { Id = 42, Nombre = "Leasing" };
            var dto     = new TipoVentaDto { Id = 42, Nombre = "Leasing" };

            _mapperMock.Setup(m => m.Map<TipoVenta>(request)).Returns(new TipoVenta());
            _repoMock.Setup(r => r.AgregarAsync(It.IsAny<TipoVenta>())).ReturnsAsync(entidad);
            _mapperMock.Setup(m => m.Map<TipoVentaDto>(entidad)).Returns(dto);

            var resultado = await _handler.Handle(request, CancellationToken.None);

            Assert.Equal(42, resultado.Id);
        }
    }
}
