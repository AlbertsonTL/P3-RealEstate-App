using AutoMapper;
using Moq;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.Features.Agentes.Handlers;
using RealEstateApp.Core.Application.Features.Agentes.Queries;
using RealEstateApp.Core.Domain.Entidades;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;
using Xunit;

namespace RealEstateApp.Tests.Unitarias.Handlers
{
    public class ObtenerPropiedadesAgenteQueryHandlerTests
    {
        private readonly Mock<IRepositorioPropiedad> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ObtenerPropiedadesAgenteQueryHandler _handler;

        public ObtenerPropiedadesAgenteQueryHandlerTests()
        {
            _repoMock   = new Mock<IRepositorioPropiedad>();
            _mapperMock = new Mock<IMapper>();
            _handler    = new ObtenerPropiedadesAgenteQueryHandler(_repoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_DebeRetornarPropiedades_CuandoAgenteExisteConPropiedades()
        {
            string agenteId = "agente-001";
            var propiedades = new List<Propiedad>
            {
                new() { Id = 1, AgenteId = agenteId, Codigo = "P001" },
                new() { Id = 2, AgenteId = agenteId, Codigo = "P002" }
            };
            var dtos = new List<PropiedadDto>
            {
                new() { Id = 1, Codigo = "P001" },
                new() { Id = 2, Codigo = "P002" }
            };

            _repoMock.Setup(r => r.ObtenerPorAgenteAsync(agenteId)).ReturnsAsync(propiedades);
            _mapperMock.Setup(m => m.Map<IEnumerable<PropiedadDto>>(propiedades)).Returns(dtos);

            var resultado = await _handler.Handle(new ObtenerPropiedadesAgenteQuery { AgenteId = agenteId }, CancellationToken.None);

            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count());
        }

        [Fact]
        public async Task Handle_DebeRetornarColeccionVacia_CuandoAgenteNoPropiedades()
        {
            string agenteId = "agente-sin-props";

            _repoMock.Setup(r => r.ObtenerPorAgenteAsync(agenteId)).ReturnsAsync(new List<Propiedad>());
            _mapperMock.Setup(m => m.Map<IEnumerable<PropiedadDto>>(It.IsAny<IEnumerable<Propiedad>>()))
                       .Returns(new List<PropiedadDto>());

            var resultado = await _handler.Handle(
                new ObtenerPropiedadesAgenteQuery { AgenteId = agenteId }, CancellationToken.None);

            Assert.Empty(resultado);
        }

        [Fact]
        public async Task Handle_DebeLlamarObtenerPorAgenteAsync_UnaVez()
        {
            string agenteId = "agente-002";
            _repoMock.Setup(r => r.ObtenerPorAgenteAsync(agenteId)).ReturnsAsync(new List<Propiedad>());
            _mapperMock.Setup(m => m.Map<IEnumerable<PropiedadDto>>(It.IsAny<IEnumerable<Propiedad>>()))
                       .Returns(new List<PropiedadDto>());

            await _handler.Handle(new ObtenerPropiedadesAgenteQuery { AgenteId = agenteId }, CancellationToken.None);

            _repoMock.Verify(r => r.ObtenerPorAgenteAsync(agenteId), Times.Once);
        }

        [Fact]
        public async Task Handle_DebeMapearPropiedadesADtos_UnaVez()
        {
            string agenteId = "agente-003";
            var propiedades = new List<Propiedad> { new() { Id = 10, Codigo = "X001" } };

            _repoMock.Setup(r => r.ObtenerPorAgenteAsync(agenteId)).ReturnsAsync(propiedades);
            _mapperMock.Setup(m => m.Map<IEnumerable<PropiedadDto>>(propiedades))
                       .Returns(new List<PropiedadDto> { new() { Id = 10 } });

            await _handler.Handle(new ObtenerPropiedadesAgenteQuery { AgenteId = agenteId }, CancellationToken.None);

            _mapperMock.Verify(m => m.Map<IEnumerable<PropiedadDto>>(propiedades), Times.Once);
        }

        [Fact]
        public async Task Handle_DebeRetornarCodigoCorrecto_EnLasPropiedades()
        {
            string agenteId = "agente-004";
            var propiedades = new List<Propiedad> { new() { Id = 5, AgenteId = agenteId, Codigo = "CASA001" } };
            var dtos        = new List<PropiedadDto> { new() { Id = 5, Codigo = "CASA001" } };

            _repoMock.Setup(r => r.ObtenerPorAgenteAsync(agenteId)).ReturnsAsync(propiedades);
            _mapperMock.Setup(m => m.Map<IEnumerable<PropiedadDto>>(propiedades)).Returns(dtos);

            var resultado = await _handler.Handle(
                new ObtenerPropiedadesAgenteQuery { AgenteId = agenteId }, CancellationToken.None);

            Assert.Equal("CASA001", resultado.First().Codigo);
        }
    }
}
