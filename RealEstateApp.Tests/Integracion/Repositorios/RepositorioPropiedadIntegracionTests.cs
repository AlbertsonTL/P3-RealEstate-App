using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Entidades;
using RealEstateApp.Infrastructure.Data.Contexto;
using RealEstateApp.Infrastructure.Data.Repositorios;
using Xunit;

namespace RealEstateApp.Tests.Integracion.Repositorios
{
    public class RepositorioPropiedadIntegracionTests
    {
        private readonly AplicacionDbContext _dbContext;
        private readonly RepositorioPropiedad _repositorio;

        public RepositorioPropiedadIntegracionTests()
        {
            var options = new DbContextOptionsBuilder<AplicacionDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new AplicacionDbContext(options);
            _repositorio = new RepositorioPropiedad(_dbContext);

            // Sembrar entidades requeridas por las FK de Propiedad.
            // ObtenerPorCodigoAsync usa Include sobre TipoPropiedad y TipoVenta
            // (navegaciones requeridas). En EF Core 10 con InMemory, si las entidades
            // relacionadas no existen, el Include aplica semántica de INNER JOIN y
            // filtra la propiedad, devolviendo null. Sembrar estos registros evita ese problema.
            _dbContext.TiposPropiedades.Add(new TipoPropiedad { Id = 1, Nombre = "Casa", Descripcion = "Vivienda unifamiliar" });
            _dbContext.TiposVentas.Add(new TipoVenta { Id = 1, Nombre = "Venta", Descripcion = "Compra-venta directa" });
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task AgregarAsync_DebeGuardarPropiedadEnBaseDeDatos()
        {
            // Arrange
            // TipoPropiedadId y TipoVentaId deben apuntar a registros existentes
            // para que ObtenerPorCodigoAsync (con Include) pueda encontrar la propiedad.
            var propiedad = new Propiedad
            {
                Precio = 100000,
                Descripcion = "Propiedad de prueba",
                AgenteId = "agente-test-id",
                FechaCreacion = DateTime.UtcNow,
                TipoPropiedadId = 1,
                TipoVentaId = 1
            };

            // Act
            // NOTA: AgregarAsync sobrescribe el Codigo con uno generado aleatoriamente,
            // por eso usamos propiedad.Codigo (modificado in-place) para la búsqueda,
            // en lugar de un valor hardcodeado que ya no existirá en la BD.
            await _repositorio.AgregarAsync(propiedad);
            var guardada = await _repositorio.ObtenerPorCodigoAsync(propiedad.Codigo);

            // Assert
            Assert.NotNull(guardada);
            Assert.Equal(propiedad.Codigo, guardada.Codigo);
            Assert.Equal(100000, guardada.Precio);
            Assert.Equal("Propiedad de prueba", guardada.Descripcion);
        }
    }
}