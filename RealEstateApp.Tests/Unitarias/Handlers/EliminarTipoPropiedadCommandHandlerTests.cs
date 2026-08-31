using MediatR;
using Moq;
using RealEstateApp.Core.Application.Features.TipoPropiedades.Commands;
using RealEstateApp.Core.Application.Features.TipoPropiedades.Handlers;
using RealEstateApp.Core.Domain.Entidades;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;
using Xunit;

namespace RealEstateApp.Tests.Unitarias.Handlers
{
    public class EliminarTipoPropiedadCommandHandlerTests
    {
        private readonly Mock<IRepositorioGenerico<TipoPropiedad>> _repoMock;
        private readonly EliminarTipoPropiedadCommandHandler _handler;

        public EliminarTipoPropiedadCommandHandlerTests()
        {
            _repoMock = new Mock<IRepositorioGenerico<TipoPropiedad>>();
            _handler = new EliminarTipoPropiedadCommandHandler(_repoMock.Object);
        }

        [Fact]
        public async Task Handle_DebeLlamarEliminar_CuandoElTipoExiste()
        {
            // Arrange
            int idAbuscar = 1;
            var comando = new EliminarTipoPropiedadCommand { Id = idAbuscar };
            var entidadSimulada = new TipoPropiedad { Id = idAbuscar, Nombre = "Casa" };

            _repoMock.Setup(r => r.ObtenerPorIdAsync(idAbuscar)).ReturnsAsync(entidadSimulada);
            _repoMock.Setup(r => r.EliminarAsync(entidadSimulada)).Returns(Task.CompletedTask);

            // Act
            var resultado = await _handler.Handle(comando, CancellationToken.None);

            // Assert
            Assert.Equal(Unit.Value, resultado);
            _repoMock.Verify(r => r.ObtenerPorIdAsync(idAbuscar), Times.Once);
            _repoMock.Verify(r => r.EliminarAsync(entidadSimulada), Times.Once);
        }

        [Fact]
        public async Task Handle_NoDebeLlamarEliminar_CuandoElTipoNoExiste()
        {
            // Arrange
            int idAbuscar = 99;
            var comando = new EliminarTipoPropiedadCommand { Id = idAbuscar };

            _repoMock.Setup(r => r.ObtenerPorIdAsync(idAbuscar)).ReturnsAsync((TipoPropiedad)null!);

            // Act
            var resultado = await _handler.Handle(comando, CancellationToken.None);

            // Assert
            Assert.Equal(Unit.Value, resultado);
            _repoMock.Verify(r => r.ObtenerPorIdAsync(idAbuscar), Times.Once);
            _repoMock.Verify(r => r.EliminarAsync(It.IsAny<TipoPropiedad>()), Times.Never);
        }
    }
}
