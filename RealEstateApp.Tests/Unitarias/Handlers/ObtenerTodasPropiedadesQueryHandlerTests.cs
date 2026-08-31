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
    public class ObtenerTodasPropiedadesQueryHandlerTests
    {
        private readonly Mock<IRepositorioPropiedad> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ObtenerTodasPropiedadesQueryHandler _handler;

        public ObtenerTodasPropiedadesQueryHandlerTests()
        {
            _repoMock   = new Mock<IRepositorioPropiedad>();
            _mapperMock = new Mock<IMapper>();
            _handler    = new ObtenerTodasPropiedadesQueryHandler(_repoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_DebeRetornarColeccion_CuandoExistenPropiedades()
        {
            var propiedades = new List<Propiedad> { new Propiedad { Id = 1, Codigo = "123456" } };
            var dtos        = new List<PropiedadDto> { new PropiedadDto { Id = 1, Codigo = "123456" } };
            _repoMock.Setup(r => r.ObtenerTodosAsync()).ReturnsAsync(propiedades);
            _mapperMock.Setup(m => m.Map<IEnumerable<PropiedadDto>>(propiedades)).Returns(dtos);

            var resultado = await _handler.Handle(new ObtenerTodasPropiedadesQuery(), CancellationToken.None);

            Assert.NotEmpty(resultado);
            Assert.Single(resultado);
            Assert.Equal("123456", resultado.First().Codigo);
        }

        [Fact]
        public async Task Handle_DebeRetornarColeccionVacia_CuandoNoExistenPropiedades()
        {
            var propiedades = new List<Propiedad>();
            var dtos        = new List<PropiedadDto>();
            _repoMock.Setup(r => r.ObtenerTodosAsync()).ReturnsAsync(propiedades);
            _mapperMock.Setup(m => m.Map<IEnumerable<PropiedadDto>>(propiedades)).Returns(dtos);

            var resultado = await _handler.Handle(new ObtenerTodasPropiedadesQuery(), CancellationToken.None);

            Assert.Empty(resultado);
        }

        [Fact]
        public async Task Handle_DebeLlamarObtenerTodosAsync_UnaVez()
        {
            var propiedades = new List<Propiedad>();
            var dtos        = new List<PropiedadDto>();
            _repoMock.Setup(r => r.ObtenerTodosAsync()).ReturnsAsync(propiedades);
            _mapperMock.Setup(m => m.Map<IEnumerable<PropiedadDto>>(propiedades)).Returns(dtos);

            await _handler.Handle(new ObtenerTodasPropiedadesQuery(), CancellationToken.None);

            _repoMock.Verify(r => r.ObtenerTodosAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_DebeRetornarMultiplesPropiedades_CuandoExistenVarias()
        {
            var propiedades = new List<Propiedad>
            {
                new Propiedad { Id = 1, Codigo = "AAA001" },
                new Propiedad { Id = 2, Codigo = "BBB002" },
                new Propiedad { Id = 3, Codigo = "CCC003" }
            };
            var dtos = propiedades.Select(p => new PropiedadDto { Id = p.Id, Codigo = p.Codigo }).ToList();
            _repoMock.Setup(r => r.ObtenerTodosAsync()).ReturnsAsync(propiedades);
            _mapperMock.Setup(m => m.Map<IEnumerable<PropiedadDto>>(propiedades)).Returns(dtos);

            var resultado = await _handler.Handle(new ObtenerTodasPropiedadesQuery(), CancellationToken.None);

            Assert.Equal(3, resultado.Count());
        }
    }
}
