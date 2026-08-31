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
    public class ActualizarTipoPropiedadCommandHandlerTests
    {
        private readonly Mock<IRepositorioGenerico<TipoPropiedad>> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ActualizarTipoPropiedadCommandHandler _handler;

        public ActualizarTipoPropiedadCommandHandlerTests()
        {
            _repoMock   = new Mock<IRepositorioGenerico<TipoPropiedad>>();
            _mapperMock = new Mock<IMapper>();
            _handler    = new ActualizarTipoPropiedadCommandHandler(_repoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_DebeActualizarYRetornarDto_CuandoExiste()
        {
            var command = new ActualizarTipoPropiedadCommand { Id = 1, Nombre = "Penthouse", Descripcion = "Suite de lujo" };
            var entidad = new TipoPropiedad { Id = 1, Nombre = "Viejo", Descripcion = "Vieja" };
            var dto     = new TipoPropiedadDto { Id = 1, Nombre = "Penthouse", Descripcion = "Suite de lujo" };

            _repoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(entidad);
            _repoMock.Setup(r => r.ActualizarAsync(It.IsAny<TipoPropiedad>())).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<TipoPropiedadDto>(It.IsAny<TipoPropiedad>())).Returns(dto);

            var resultado = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(resultado);
            Assert.Equal("Penthouse", resultado.Nombre);
            _repoMock.Verify(r => r.ActualizarAsync(It.IsAny<TipoPropiedad>()), Times.Once);
        }

        [Fact]
        public async Task Handle_DebeRetornarNull_CuandoNoExiste()
        {
            var command = new ActualizarTipoPropiedadCommand { Id = 999, Nombre = "X", Descripcion = "Y" };

            _repoMock.Setup(r => r.ObtenerPorIdAsync(999)).ReturnsAsync((TipoPropiedad?)null);

            var resultado = await _handler.Handle(command, CancellationToken.None);

            Assert.Null(resultado);
            _repoMock.Verify(r => r.ActualizarAsync(It.IsAny<TipoPropiedad>()), Times.Never);
        }

        [Fact]
        public async Task Handle_DebeModificarCampos_EnLaEntidad()
        {
            var command = new ActualizarTipoPropiedadCommand { Id = 2, Nombre = "Villa", Descripcion = "Villa privada" };
            var entidad = new TipoPropiedad { Id = 2, Nombre = "Viejo", Descripcion = "Vieja" };

            _repoMock.Setup(r => r.ObtenerPorIdAsync(2)).ReturnsAsync(entidad);
            _repoMock.Setup(r => r.ActualizarAsync(It.IsAny<TipoPropiedad>())).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<TipoPropiedadDto>(It.IsAny<TipoPropiedad>())).Returns(new TipoPropiedadDto { Id = 2 });

            await _handler.Handle(command, CancellationToken.None);

            Assert.Equal("Villa",         entidad.Nombre);
            Assert.Equal("Villa privada", entidad.Descripcion);
        }

        [Fact]
        public async Task Handle_DebeLlamarObtenerPorId_UnaVez()
        {
            var command = new ActualizarTipoPropiedadCommand { Id = 3, Nombre = "Loft", Descripcion = "Loft moderno" };
            var entidad = new TipoPropiedad { Id = 3, Nombre = "Viejo" };

            _repoMock.Setup(r => r.ObtenerPorIdAsync(3)).ReturnsAsync(entidad);
            _repoMock.Setup(r => r.ActualizarAsync(entidad)).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<TipoPropiedadDto>(entidad)).Returns(new TipoPropiedadDto { Id = 3 });

            await _handler.Handle(command, CancellationToken.None);

            _repoMock.Verify(r => r.ObtenerPorIdAsync(3), Times.Once);
        }
    }
}
