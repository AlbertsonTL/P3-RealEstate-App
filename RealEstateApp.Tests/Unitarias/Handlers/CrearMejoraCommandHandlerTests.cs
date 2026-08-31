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
    public class CrearMejoraCommandHandlerTests
    {
        private readonly Mock<IRepositorioGenerico<Mejora>> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CrearMejoraCommandHandler _handler;

        public CrearMejoraCommandHandlerTests()
        {
            _repoMock   = new Mock<IRepositorioGenerico<Mejora>>();
            _mapperMock = new Mock<IMapper>();
            _handler    = new CrearMejoraCommandHandler(_repoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_DebeRetornarMejoraDto_CuandoSeCreaExitosamente()
        {
            var request = new CrearMejoraCommand { Nombre = "Piscina", Descripcion = "Piscina olímpica" };
            var entidad = new Mejora { Id = 1, Nombre = "Piscina", Descripcion = "Piscina olímpica" };
            var dto     = new MejoraDto { Id = 1, Nombre = "Piscina", Descripcion = "Piscina olímpica" };

            _mapperMock.Setup(m => m.Map<Mejora>(request)).Returns(new Mejora { Nombre = "Piscina" });
            _repoMock.Setup(r => r.AgregarAsync(It.IsAny<Mejora>())).ReturnsAsync(entidad);
            _mapperMock.Setup(m => m.Map<MejoraDto>(entidad)).Returns(dto);

            var resultado = await _handler.Handle(request, CancellationToken.None);

            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
            Assert.Equal("Piscina", resultado.Nombre);
            _repoMock.Verify(r => r.AgregarAsync(It.IsAny<Mejora>()), Times.Once);
        }

        [Fact]
        public async Task Handle_DebeMapearComandoAEntidad_AntesDeAgregar()
        {
            var request = new CrearMejoraCommand { Nombre = "Garaje", Descripcion = "Garaje doble" };
            var entidad = new Mejora { Id = 2, Nombre = "Garaje" };
            var dto     = new MejoraDto { Id = 2, Nombre = "Garaje" };

            _mapperMock.Setup(m => m.Map<Mejora>(request)).Returns(new Mejora { Nombre = "Garaje" });
            _repoMock.Setup(r => r.AgregarAsync(It.IsAny<Mejora>())).ReturnsAsync(entidad);
            _mapperMock.Setup(m => m.Map<MejoraDto>(entidad)).Returns(dto);

            await _handler.Handle(request, CancellationToken.None);

            _mapperMock.Verify(m => m.Map<Mejora>(request), Times.Once);
        }

        [Fact]
        public async Task Handle_DebeRetornarDto_ConIdGeneradoPorRepositorio()
        {
            var request = new CrearMejoraCommand { Nombre = "Jacuzzi", Descripcion = "Jacuzzi exterior" };
            var entidad = new Mejora { Id = 77, Nombre = "Jacuzzi" };
            var dto     = new MejoraDto { Id = 77, Nombre = "Jacuzzi" };

            _mapperMock.Setup(m => m.Map<Mejora>(request)).Returns(new Mejora());
            _repoMock.Setup(r => r.AgregarAsync(It.IsAny<Mejora>())).ReturnsAsync(entidad);
            _mapperMock.Setup(m => m.Map<MejoraDto>(entidad)).Returns(dto);

            var resultado = await _handler.Handle(request, CancellationToken.None);

            Assert.Equal(77, resultado.Id);
        }

        [Theory]
        [InlineData("Ascensor",  "Ascensor privado")]
        [InlineData("Terraza",   "Terraza panorámica")]
        [InlineData("Gimnasio",  "Gimnasio equipado")]
        public async Task Handle_DebeCrearMejora_ConDistintosNombres(string nombre, string descripcion)
        {
            var request = new CrearMejoraCommand { Nombre = nombre, Descripcion = descripcion };
            var entidad = new Mejora { Id = 1, Nombre = nombre, Descripcion = descripcion };
            var dto     = new MejoraDto { Id = 1, Nombre = nombre, Descripcion = descripcion };

            _mapperMock.Setup(m => m.Map<Mejora>(request)).Returns(new Mejora { Nombre = nombre });
            _repoMock.Setup(r => r.AgregarAsync(It.IsAny<Mejora>())).ReturnsAsync(entidad);
            _mapperMock.Setup(m => m.Map<MejoraDto>(entidad)).Returns(dto);

            var resultado = await _handler.Handle(request, CancellationToken.None);

            Assert.Equal(nombre, resultado.Nombre);
        }
    }
}
