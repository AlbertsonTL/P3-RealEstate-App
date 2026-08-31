using AutoMapper;
using Moq;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.Features.TipoPropiedades.Handlers;
using RealEstateApp.Core.Application.Features.TipoPropiedades.Queries;
using RealEstateApp.Core.Domain.Entidades;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;
using Xunit;

namespace RealEstateApp.Tests.Unitarias.Handlers
{
    public class ObtenerTiposPropiedadesQueryHandlerTests
    {
        private readonly Mock<IRepositorioGenerico<TipoPropiedad>> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ObtenerTiposPropiedadesQueryHandler _handler;

        public ObtenerTiposPropiedadesQueryHandlerTests()
        {
            _repoMock   = new Mock<IRepositorioGenerico<TipoPropiedad>>();
            _mapperMock = new Mock<IMapper>();
            _handler    = new ObtenerTiposPropiedadesQueryHandler(_repoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_DebeRetornarColeccion_CuandoExistesTipos()
        {
            var entidades = new List<TipoPropiedad>
            {
                new() { Id = 1, Nombre = "Casa",        Descripcion = "Vivienda unifamiliar" },
                new() { Id = 2, Nombre = "Apartamento", Descripcion = "Unidad residencial" }
            };
            var dtos = new List<TipoPropiedadDto>
            {
                new() { Id = 1, Nombre = "Casa" },
                new() { Id = 2, Nombre = "Apartamento" }
            };

            _repoMock.Setup(r => r.ObtenerTodosAsync()).ReturnsAsync(entidades);
            _mapperMock.Setup(m => m.Map<IEnumerable<TipoPropiedadDto>>(entidades)).Returns(dtos);

            var resultado = await _handler.Handle(new ObtenerTiposPropiedadesQuery(), CancellationToken.None);

            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count());
        }

        [Fact]
        public async Task Handle_DebeRetornarColeccionVacia_CuandoNoHayTipos()
        {
            var entidades = new List<TipoPropiedad>();
            var dtos      = new List<TipoPropiedadDto>();

            _repoMock.Setup(r => r.ObtenerTodosAsync()).ReturnsAsync(entidades);
            _mapperMock.Setup(m => m.Map<IEnumerable<TipoPropiedadDto>>(entidades)).Returns(dtos);

            var resultado = await _handler.Handle(new ObtenerTiposPropiedadesQuery(), CancellationToken.None);

            Assert.Empty(resultado);
        }

        [Fact]
        public async Task Handle_DebeLlamarObtenerTodosAsync_UnaVez()
        {
            var entidades = new List<TipoPropiedad>();
            _repoMock.Setup(r => r.ObtenerTodosAsync()).ReturnsAsync(entidades);
            _mapperMock.Setup(m => m.Map<IEnumerable<TipoPropiedadDto>>(entidades)).Returns(new List<TipoPropiedadDto>());

            await _handler.Handle(new ObtenerTiposPropiedadesQuery(), CancellationToken.None);

            _repoMock.Verify(r => r.ObtenerTodosAsync(), Times.Once);
        }
    }

    public class ObtenerTipoPropiedadPorIdQueryHandlerTests
    {
        private readonly Mock<IRepositorioGenerico<TipoPropiedad>> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ObtenerTipoPropiedadPorIdQueryHandler _handler;

        public ObtenerTipoPropiedadPorIdQueryHandlerTests()
        {
            _repoMock   = new Mock<IRepositorioGenerico<TipoPropiedad>>();
            _mapperMock = new Mock<IMapper>();
            _handler    = new ObtenerTipoPropiedadPorIdQueryHandler(_repoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_DebeRetornarDto_CuandoExiste()
        {
            var entidad = new TipoPropiedad { Id = 5, Nombre = "Villa", Descripcion = "Villa privada" };
            var dto     = new TipoPropiedadDto { Id = 5, Nombre = "Villa", Descripcion = "Villa privada" };

            _repoMock.Setup(r => r.ObtenerPorIdAsync(5)).ReturnsAsync(entidad);
            _mapperMock.Setup(m => m.Map<TipoPropiedadDto>(entidad)).Returns(dto);

            var resultado = await _handler.Handle(new ObtenerTipoPropiedadPorIdQuery { Id = 5 }, CancellationToken.None);

            Assert.NotNull(resultado);
            Assert.Equal(5,       resultado.Id);
            Assert.Equal("Villa", resultado.Nombre);
        }

        [Fact]
        public async Task Handle_DebeRetornarNull_CuandoNoExiste()
        {
            _repoMock.Setup(r => r.ObtenerPorIdAsync(999)).ReturnsAsync((TipoPropiedad?)null);
            _mapperMock.Setup(m => m.Map<TipoPropiedadDto>(null!)).Returns((TipoPropiedadDto?)null!);

            var resultado = await _handler.Handle(new ObtenerTipoPropiedadPorIdQuery { Id = 999 }, CancellationToken.None);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task Handle_DebeLlamarObtenerPorIdAsync_UnaVez()
        {
            _repoMock.Setup(r => r.ObtenerPorIdAsync(3)).ReturnsAsync((TipoPropiedad?)null);
            _mapperMock.Setup(m => m.Map<TipoPropiedadDto>(null!)).Returns((TipoPropiedadDto?)null!);

            await _handler.Handle(new ObtenerTipoPropiedadPorIdQuery { Id = 3 }, CancellationToken.None);

            _repoMock.Verify(r => r.ObtenerPorIdAsync(3), Times.Once);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(25)]
        public async Task Handle_DebeUsarIdCorrectamente_ConDistintosIds(int id)
        {
            var entidad = new TipoPropiedad { Id = id, Nombre = $"Tipo{id}" };
            var dto     = new TipoPropiedadDto { Id = id, Nombre = $"Tipo{id}" };

            _repoMock.Setup(r => r.ObtenerPorIdAsync(id)).ReturnsAsync(entidad);
            _mapperMock.Setup(m => m.Map<TipoPropiedadDto>(entidad)).Returns(dto);

            var resultado = await _handler.Handle(new ObtenerTipoPropiedadPorIdQuery { Id = id }, CancellationToken.None);

            Assert.Equal(id, resultado.Id);
        }
    }
}
