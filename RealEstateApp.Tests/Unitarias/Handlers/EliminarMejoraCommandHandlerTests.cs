using MediatR;
using Moq;
using RealEstateApp.Core.Application.Features.Mejoras.Commands;
using RealEstateApp.Core.Application.Features.Mejoras.Handlers;
using RealEstateApp.Core.Domain.Entidades;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;
using Xunit;

namespace RealEstateApp.Tests.Unitarias.Handlers
{
    public class EliminarMejoraCommandHandlerTests
    {
        private readonly Mock<IRepositorioGenerico<Mejora>> _repoMock;
        private readonly EliminarMejoraCommandHandler _handler;

        public EliminarMejoraCommandHandlerTests()
        {
            _repoMock = new Mock<IRepositorioGenerico<Mejora>>();
            _handler  = new EliminarMejoraCommandHandler(_repoMock.Object);
        }

        [Fact]
        public async Task Handle_DebeLlamarEliminar_CuandoMejoraExiste()
        {
            int id      = 1;
            var entidad = new Mejora { Id = id, Nombre = "Piscina" };

            _repoMock.Setup(r => r.ObtenerPorIdAsync(id)).ReturnsAsync(entidad);
            _repoMock.Setup(r => r.EliminarAsync(entidad)).Returns(Task.CompletedTask);

            var resultado = await _handler.Handle(new EliminarMejoraCommand { Id = id }, CancellationToken.None);

            Assert.Equal(Unit.Value, resultado);
            _repoMock.Verify(r => r.ObtenerPorIdAsync(id),    Times.Once);
            _repoMock.Verify(r => r.EliminarAsync(entidad),   Times.Once);
        }

        [Fact]
        public async Task Handle_NoDebeLlamarEliminar_CuandoMejoraNoExiste()
        {
            int id = 999;
            _repoMock.Setup(r => r.ObtenerPorIdAsync(id)).ReturnsAsync((Mejora?)null);

            var resultado = await _handler.Handle(new EliminarMejoraCommand { Id = id }, CancellationToken.None);

            Assert.Equal(Unit.Value, resultado);
            _repoMock.Verify(r => r.EliminarAsync(It.IsAny<Mejora>()), Times.Never);
        }

        [Fact]
        public async Task Handle_DebeRetornarUnitValue_CuandoMejoraExiste()
        {
            var entidad = new Mejora { Id = 5, Nombre = "Cancha" };
            _repoMock.Setup(r => r.ObtenerPorIdAsync(5)).ReturnsAsync(entidad);
            _repoMock.Setup(r => r.EliminarAsync(entidad)).Returns(Task.CompletedTask);

            var resultado = await _handler.Handle(new EliminarMejoraCommand { Id = 5 }, CancellationToken.None);

            Assert.Equal(Unit.Value, resultado);
        }

        [Fact]
        public async Task Handle_DebeRetornarUnitValue_CuandoMejoraNoExiste()
        {
            _repoMock.Setup(r => r.ObtenerPorIdAsync(It.IsAny<int>())).ReturnsAsync((Mejora?)null);

            var resultado = await _handler.Handle(new EliminarMejoraCommand { Id = 42 }, CancellationToken.None);

            Assert.Equal(Unit.Value, resultado);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(100)]
        public async Task Handle_DebeLlamarObtenerPorId_UnaVez_ConCualquierId(int id)
        {
            _repoMock.Setup(r => r.ObtenerPorIdAsync(id)).ReturnsAsync((Mejora?)null);

            await _handler.Handle(new EliminarMejoraCommand { Id = id }, CancellationToken.None);

            _repoMock.Verify(r => r.ObtenerPorIdAsync(id), Times.Once);
        }
    }
}
