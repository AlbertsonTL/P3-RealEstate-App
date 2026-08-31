using MediatR;
using Moq;
using RealEstateApp.Core.Application.Features.TipoVentas.Commands;
using RealEstateApp.Core.Application.Features.TipoVentas.Handlers;
using RealEstateApp.Core.Domain.Entidades;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;
using Xunit;

namespace RealEstateApp.Tests.Unitarias.Handlers
{
    public class EliminarTipoVentaCommandHandlerTests
    {
        private readonly Mock<IRepositorioGenerico<TipoVenta>> _repoMock;
        private readonly EliminarTipoVentaCommandHandler _handler;

        public EliminarTipoVentaCommandHandlerTests()
        {
            _repoMock = new Mock<IRepositorioGenerico<TipoVenta>>();
            _handler  = new EliminarTipoVentaCommandHandler(_repoMock.Object);
        }

        [Fact]
        public async Task Handle_DebeLlamarEliminar_CuandoElTipoExiste()
        {
            int id      = 1;
            var entidad = new TipoVenta { Id = id, Nombre = "Alquiler" };
            _repoMock.Setup(r => r.ObtenerPorIdAsync(id)).ReturnsAsync(entidad);
            _repoMock.Setup(r => r.EliminarAsync(entidad)).Returns(Task.CompletedTask);

            var resultado = await _handler.Handle(new EliminarTipoVentaCommand { Id = id }, CancellationToken.None);

            Assert.Equal(Unit.Value, resultado);
            _repoMock.Verify(r => r.EliminarAsync(entidad), Times.Once);
        }

        [Fact]
        public async Task Handle_NoDebeLlamarEliminar_CuandoElTipoNoExiste()
        {
            int id = 99;
            _repoMock.Setup(r => r.ObtenerPorIdAsync(id)).ReturnsAsync((TipoVenta?)null);

            var resultado = await _handler.Handle(new EliminarTipoVentaCommand { Id = id }, CancellationToken.None);

            Assert.Equal(Unit.Value, resultado);
            _repoMock.Verify(r => r.EliminarAsync(It.IsAny<TipoVenta>()), Times.Never);
        }

        [Fact]
        public async Task Handle_DebeLlamarObtenerPorIdAsync_UnaVez()
        {
            int id = 5;
            _repoMock.Setup(r => r.ObtenerPorIdAsync(id)).ReturnsAsync((TipoVenta?)null);

            await _handler.Handle(new EliminarTipoVentaCommand { Id = id }, CancellationToken.None);

            _repoMock.Verify(r => r.ObtenerPorIdAsync(id), Times.Once);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(10)]
        public async Task Handle_DebeRetornarUnitValue_SiempreIndependienteDeLaExistencia(int id)
        {
            _repoMock.Setup(r => r.ObtenerPorIdAsync(id)).ReturnsAsync((TipoVenta?)null);

            var resultado = await _handler.Handle(new EliminarTipoVentaCommand { Id = id }, CancellationToken.None);

            Assert.Equal(Unit.Value, resultado);
        }
    }
}
