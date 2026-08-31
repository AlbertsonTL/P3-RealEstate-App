using AutoMapper;
using Moq;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.Features.Mejoras.Commands;
using RealEstateApp.Core.Application.Features.Mejoras.Handlers;
using RealEstateApp.Core.Domain.Entidades;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;
using Xunit;

namespace RealEstateApp.Tests.Unitarias.Handlers
{
    public class ActualizarMejoraCommandHandlerTests
    {
        private readonly Mock<IRepositorioGenerico<Mejora>> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ActualizarMejoraCommandHandler _handler;

        public ActualizarMejoraCommandHandlerTests()
        {
            _repoMock   = new Mock<IRepositorioGenerico<Mejora>>();
            _mapperMock = new Mock<IMapper>();
            _handler    = new ActualizarMejoraCommandHandler(_repoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_DebeActualizarYRetornarDto_CuandoMejoraExiste()
        {
            var command = new ActualizarMejoraCommand { Id = 1, Nombre = "Piscina Actualizada", Descripcion = "Nueva desc" };
            var entidad = new Mejora { Id = 1, Nombre = "Piscina Vieja", Descripcion = "Desc vieja" };
            var dto     = new MejoraDto { Id = 1, Nombre = "Piscina Actualizada", Descripcion = "Nueva desc" };

            _repoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(entidad);
            _repoMock.Setup(r => r.ActualizarAsync(It.IsAny<Mejora>())).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<MejoraDto>(It.IsAny<Mejora>())).Returns(dto);

            var resultado = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(resultado);
            Assert.Equal("Piscina Actualizada", resultado.Nombre);
            _repoMock.Verify(r => r.ActualizarAsync(It.IsAny<Mejora>()), Times.Once);
        }

        [Fact]
        public async Task Handle_DebeRetornarNull_CuandoMejoraNoExiste()
        {
            var command = new ActualizarMejoraCommand { Id = 999, Nombre = "Inexistente", Descripcion = "Nada" };

            _repoMock.Setup(r => r.ObtenerPorIdAsync(999)).ReturnsAsync((Mejora?)null);

            var resultado = await _handler.Handle(command, CancellationToken.None);

            Assert.Null(resultado);
            _repoMock.Verify(r => r.ActualizarAsync(It.IsAny<Mejora>()), Times.Never);
        }

        [Fact]
        public async Task Handle_DebeModificarNombreYDescripcion_EnLaEntidad()
        {
            var command = new ActualizarMejoraCommand { Id = 5, Nombre = "Sauna", Descripcion = "Sauna finlandesa" };
            var entidad = new Mejora { Id = 5, Nombre = "Viejo", Descripcion = "Vieja" };

            _repoMock.Setup(r => r.ObtenerPorIdAsync(5)).ReturnsAsync(entidad);
            _repoMock.Setup(r => r.ActualizarAsync(It.IsAny<Mejora>())).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<MejoraDto>(It.IsAny<Mejora>())).Returns(new MejoraDto { Id = 5 });

            await _handler.Handle(command, CancellationToken.None);

            // Verifica que el handler mutó los campos de la entidad antes de actualizar
            Assert.Equal("Sauna",            entidad.Nombre);
            Assert.Equal("Sauna finlandesa", entidad.Descripcion);
        }

        [Fact]
        public async Task Handle_DebeLlamarObtenerPorId_AntesDeActualizar()
        {
            var command = new ActualizarMejoraCommand { Id = 3, Nombre = "Cancha", Descripcion = "Cancha de tenis" };
            var entidad = new Mejora { Id = 3, Nombre = "Viejo" };

            _repoMock.Setup(r => r.ObtenerPorIdAsync(3)).ReturnsAsync(entidad);
            _repoMock.Setup(r => r.ActualizarAsync(entidad)).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<MejoraDto>(entidad)).Returns(new MejoraDto { Id = 3 });

            await _handler.Handle(command, CancellationToken.None);

            _repoMock.Verify(r => r.ObtenerPorIdAsync(3),          Times.Once);
            _repoMock.Verify(r => r.ActualizarAsync(It.IsAny<Mejora>()), Times.Once);
        }

        [Theory]
        [InlineData(1, "Balcón",   "Balcón amplio")]
        [InlineData(2, "Bodega",   "Bodega de 20m2")]
        [InlineData(3, "Seguridad","Seguridad 24h")]
        public async Task Handle_DebeActualizarCampos_ConDistintosValores(int id, string nombre, string descripcion)
        {
            var command = new ActualizarMejoraCommand { Id = id, Nombre = nombre, Descripcion = descripcion };
            var entidad = new Mejora { Id = id, Nombre = "Viejo", Descripcion = "Vieja" };

            _repoMock.Setup(r => r.ObtenerPorIdAsync(id)).ReturnsAsync(entidad);
            _repoMock.Setup(r => r.ActualizarAsync(It.IsAny<Mejora>())).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<MejoraDto>(It.IsAny<Mejora>())).Returns(new MejoraDto { Id = id, Nombre = nombre });

            var resultado = await _handler.Handle(command, CancellationToken.None);

            Assert.Equal(nombre, entidad.Nombre);
            Assert.Equal(descripcion, entidad.Descripcion);
        }
    }
}
