using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Entidades;
using RealEstateApp.Infrastructure.Data.Contexto;
using RealEstateApp.Infrastructure.Data.Repositorios;
using Xunit;

namespace RealEstateApp.Tests.Integracion.Repositorios
{
    /// <summary>
    /// Pruebas de integración para RepositorioGenerico usando Mejora como entidad.
    /// Usa EF Core InMemory para aislar la BD real.
    /// </summary>
    public class RepositorioMejoraIntegracionTests
    {
        private readonly AplicacionDbContext _dbContext;
        private readonly RepositorioGenerico<Mejora> _repositorio;

        public RepositorioMejoraIntegracionTests()
        {
            var options = new DbContextOptionsBuilder<AplicacionDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext   = new AplicacionDbContext(options);
            _repositorio = new RepositorioGenerico<Mejora>(_dbContext);
        }

        [Fact]
        public async Task AgregarAsync_DebeGuardarMejora_YObtenerseCorrectamente()
        {
            var mejora = new Mejora { Nombre = "Piscina", Descripcion = "Piscina olímpica" };

            var agregada = await _repositorio.AgregarAsync(mejora);
            var obtenida = await _repositorio.ObtenerPorIdAsync(agregada.Id);

            Assert.NotNull(obtenida);
            Assert.Equal("Piscina",          obtenida.Nombre);
            Assert.Equal("Piscina olímpica", obtenida.Descripcion);
            Assert.True(agregada.Id > 0);
        }

        [Fact]
        public async Task ObtenerTodosAsync_DebeRetornarTodasLasMejoras()
        {
            await _repositorio.AgregarAsync(new Mejora { Nombre = "Garaje",   Descripcion = "Garaje doble" });
            await _repositorio.AgregarAsync(new Mejora { Nombre = "Terraza",  Descripcion = "Terraza amplia" });
            await _repositorio.AgregarAsync(new Mejora { Nombre = "Ascensor", Descripcion = "Ascensor privado" });

            var todas = await _repositorio.ObtenerTodosAsync();

            Assert.Equal(3, todas.Count());
        }

        [Fact]
        public async Task ObtenerTodosAsync_DebeRetornarColeccionVacia_CuandoNoHayMejoras()
        {
            var todas = await _repositorio.ObtenerTodosAsync();

            Assert.Empty(todas);
        }

        [Fact]
        public async Task ActualizarAsync_DebeModificarLaMejora_EnBaseDeDatos()
        {
            var mejora = await _repositorio.AgregarAsync(
                new Mejora { Nombre = "Vieja", Descripcion = "Desc vieja" });

            mejora.Nombre      = "Nueva";
            mejora.Descripcion = "Desc nueva";
            await _repositorio.ActualizarAsync(mejora);

            var actualizada = await _repositorio.ObtenerPorIdAsync(mejora.Id);
            Assert.Equal("Nueva",      actualizada!.Nombre);
            Assert.Equal("Desc nueva", actualizada.Descripcion);
        }

        [Fact]
        public async Task EliminarAsync_DebeRemoverLaMejora_DeBaseDeDatos()
        {
            var mejora = await _repositorio.AgregarAsync(
                new Mejora { Nombre = "AEliminar", Descripcion = "Borrar" });
            var id = mejora.Id;

            await _repositorio.EliminarAsync(mejora);
            var eliminada = await _repositorio.ObtenerPorIdAsync(id);

            Assert.Null(eliminada);
        }

        [Fact]
        public async Task ObtenerPorIdAsync_DebeRetornarNull_CuandoIdNoExiste()
        {
            var resultado = await _repositorio.ObtenerPorIdAsync(99999);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task AgregarAsync_DebeAsignarIdAutoincremental_CuandoSeCreaMultiplesMejoras()
        {
            var m1 = await _repositorio.AgregarAsync(new Mejora { Nombre = "M1", Descripcion = "D1" });
            var m2 = await _repositorio.AgregarAsync(new Mejora { Nombre = "M2", Descripcion = "D2" });

            Assert.NotEqual(m1.Id, m2.Id);
            Assert.True(m2.Id > m1.Id);
        }
    }
}
