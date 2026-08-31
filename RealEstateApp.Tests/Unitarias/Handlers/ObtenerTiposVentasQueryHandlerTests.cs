using AutoMapper;
using Moq;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.Features.TipoVentas.Commands;
using RealEstateApp.Core.Application.Features.TipoVentas.Handlers;
using RealEstateApp.Core.Application.Features.TipoVentas.Queries;
using RealEstateApp.Core.Domain.Entidades;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;
using Xunit;

namespace RealEstateApp.Tests.Unitarias.Handlers
{
    public class ActualizarTipoVentaCommandHandlerTests
    {
        private readonly Mock<IRepositorioGenerico<TipoVenta>> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ActualizarTipoVentaCommandHandler _handler;

        public ActualizarTipoVentaCommandHandlerTests()
        {
            _repoMock   = new Mock<IRepositorioGenerico<TipoVenta>>();
            _mapperMock = new Mock<IMapper>();
            _handler    = new ActualizarTipoVentaCommandHandler(_repoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_DebeActualizarYRetornarDto_CuandoExiste()
        {
            var command = new ActualizarTipoVentaCommand { Id = 1, Nombre = "Leasing", Descripcion = "Arrendamiento" };
            var entidad = new TipoVenta { Id = 1, Nombre = "Viejo", Descripcion = "Vieja" };
            var dto     = new TipoVentaDto { Id = 1, Nombre = "Leasing", Descripcion = "Arrendamiento" };

            _repoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(entidad);
            _repoMock.Setup(r => r.ActualizarAsync(It.IsAny<TipoVenta>())).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<TipoVentaDto>(It.IsAny<TipoVenta>())).Returns(dto);

            var resultado = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(resultado);
            Assert.Equal("Leasing", resultado.Nombre);
            _repoMock.Verify(r => r.ActualizarAsync(It.IsAny<TipoVenta>()), Times.Once);
        }

        [Fact]
        public async Task Handle_DebeRetornarNull_CuandoNoExiste()
        {
            var command = new ActualizarTipoVentaCommand { Id = 999, Nombre = "X", Descripcion = "Y" };

            _repoMock.Setup(r => r.ObtenerPorIdAsync(999)).ReturnsAsync((TipoVenta?)null);

            var resultado = await _handler.Handle(command, CancellationToken.None);

            Assert.Null(resultado);
            _repoMock.Verify(r => r.ActualizarAsync(It.IsAny<TipoVenta>()), Times.Never);
        }

        [Fact]
        public async Task Handle_DebeModificarCampos_EnLaEntidad()
        {
            var command = new ActualizarTipoVentaCommand { Id = 2, Nombre = "Alquiler VIP", Descripcion = "Alquiler premium" };
            var entidad = new TipoVenta { Id = 2, Nombre = "Viejo", Descripcion = "Vieja" };

            _repoMock.Setup(r => r.ObtenerPorIdAsync(2)).ReturnsAsync(entidad);
            _repoMock.Setup(r => r.ActualizarAsync(It.IsAny<TipoVenta>())).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<TipoVentaDto>(It.IsAny<TipoVenta>())).Returns(new TipoVentaDto { Id = 2 });

            await _handler.Handle(command, CancellationToken.None);

            Assert.Equal("Alquiler VIP",     entidad.Nombre);
            Assert.Equal("Alquiler premium", entidad.Descripcion);
        }

        [Fact]
        public async Task Handle_DebeLlamarObtenerPorId_UnaVez()
        {
            var command = new ActualizarTipoVentaCommand { Id = 3, Nombre = "Venta", Descripcion = "Compraventa" };
            var entidad = new TipoVenta { Id = 3, Nombre = "Viejo" };

            _repoMock.Setup(r => r.ObtenerPorIdAsync(3)).ReturnsAsync(entidad);
            _repoMock.Setup(r => r.ActualizarAsync(entidad)).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<TipoVentaDto>(entidad)).Returns(new TipoVentaDto { Id = 3 });

            await _handler.Handle(command, CancellationToken.None);

            _repoMock.Verify(r => r.ObtenerPorIdAsync(3), Times.Once);
        }
    }

    public class ObtenerTiposVentasQueryHandlerTests
    {
        private readonly Mock<IRepositorioGenerico<TipoVenta>> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ObtenerTiposVentasQueryHandler _handler;

        public ObtenerTiposVentasQueryHandlerTests()
        {
            _repoMock   = new Mock<IRepositorioGenerico<TipoVenta>>();
            _mapperMock = new Mock<IMapper>();
            _handler    = new ObtenerTiposVentasQueryHandler(_repoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_DebeRetornarColeccion_CuandoExistenTipos()
        {
            var entidades = new List<TipoVenta>
            {
                new() { Id = 1, Nombre = "Venta",   Descripcion = "Compraventa directa" },
                new() { Id = 2, Nombre = "Alquiler", Descripcion = "Arrendamiento mensual" }
            };
            var dtos = new List<TipoVentaDto>
            {
                new() { Id = 1, Nombre = "Venta" },
                new() { Id = 2, Nombre = "Alquiler" }
            };

            _repoMock.Setup(r => r.ObtenerTodosAsync()).ReturnsAsync(entidades);
            _mapperMock.Setup(m => m.Map<IEnumerable<TipoVentaDto>>(entidades)).Returns(dtos);

            var resultado = await _handler.Handle(new ObtenerTiposVentasQuery(), CancellationToken.None);

            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count());
        }

        [Fact]
        public async Task Handle_DebeRetornarColeccionVacia_CuandoNoHayTipos()
        {
            var entidades = new List<TipoVenta>();
            _repoMock.Setup(r => r.ObtenerTodosAsync()).ReturnsAsync(entidades);
            _mapperMock.Setup(m => m.Map<IEnumerable<TipoVentaDto>>(entidades)).Returns(new List<TipoVentaDto>());

            var resultado = await _handler.Handle(new ObtenerTiposVentasQuery(), CancellationToken.None);

            Assert.Empty(resultado);
        }

        [Fact]
        public async Task Handle_DebeLlamarObtenerTodosAsync_UnaVez()
        {
            var entidades = new List<TipoVenta>();
            _repoMock.Setup(r => r.ObtenerTodosAsync()).ReturnsAsync(entidades);
            _mapperMock.Setup(m => m.Map<IEnumerable<TipoVentaDto>>(entidades)).Returns(new List<TipoVentaDto>());

            await _handler.Handle(new ObtenerTiposVentasQuery(), CancellationToken.None);

            _repoMock.Verify(r => r.ObtenerTodosAsync(), Times.Once);
        }
    }

    public class ObtenerTipoVentaPorIdQueryHandlerTests
    {
        private readonly Mock<IRepositorioGenerico<TipoVenta>> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ObtenerTipoVentaPorIdQueryHandler _handler;

        public ObtenerTipoVentaPorIdQueryHandlerTests()
        {
            _repoMock   = new Mock<IRepositorioGenerico<TipoVenta>>();
            _mapperMock = new Mock<IMapper>();
            _handler    = new ObtenerTipoVentaPorIdQueryHandler(_repoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_DebeRetornarDto_CuandoExiste()
        {
            var entidad = new TipoVenta { Id = 4, Nombre = "Leasing", Descripcion = "Financiero" };
            var dto     = new TipoVentaDto { Id = 4, Nombre = "Leasing", Descripcion = "Financiero" };

            _repoMock.Setup(r => r.ObtenerPorIdAsync(4)).ReturnsAsync(entidad);
            _mapperMock.Setup(m => m.Map<TipoVentaDto>(entidad)).Returns(dto);

            var resultado = await _handler.Handle(new ObtenerTipoVentaPorIdQuery { Id = 4 }, CancellationToken.None);

            Assert.NotNull(resultado);
            Assert.Equal(4,         resultado.Id);
            Assert.Equal("Leasing", resultado.Nombre);
        }

        [Fact]
        public async Task Handle_DebeRetornarNull_CuandoNoExiste()
        {
            _repoMock.Setup(r => r.ObtenerPorIdAsync(999)).ReturnsAsync((TipoVenta?)null);
            _mapperMock.Setup(m => m.Map<TipoVentaDto>(null!)).Returns((TipoVentaDto?)null!);

            var resultado = await _handler.Handle(new ObtenerTipoVentaPorIdQuery { Id = 999 }, CancellationToken.None);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task Handle_DebeLlamarObtenerPorIdAsync_UnaVez()
        {
            _repoMock.Setup(r => r.ObtenerPorIdAsync(7)).ReturnsAsync((TipoVenta?)null);
            _mapperMock.Setup(m => m.Map<TipoVentaDto>(null!)).Returns((TipoVentaDto?)null!);

            await _handler.Handle(new ObtenerTipoVentaPorIdQuery { Id = 7 }, CancellationToken.None);

            _repoMock.Verify(r => r.ObtenerPorIdAsync(7), Times.Once);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(20)]
        public async Task Handle_DebeUsarIdCorrectamente_ConDistintosIds(int id)
        {
            var entidad = new TipoVenta { Id = id, Nombre = $"Tipo{id}" };
            var dto     = new TipoVentaDto { Id = id, Nombre = $"Tipo{id}" };

            _repoMock.Setup(r => r.ObtenerPorIdAsync(id)).ReturnsAsync(entidad);
            _mapperMock.Setup(m => m.Map<TipoVentaDto>(entidad)).Returns(dto);

            var resultado = await _handler.Handle(new ObtenerTipoVentaPorIdQuery { Id = id }, CancellationToken.None);

            Assert.Equal(id, resultado.Id);
        }
    }
}
