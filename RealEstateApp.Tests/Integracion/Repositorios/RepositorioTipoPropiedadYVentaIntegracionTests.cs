using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Entidades;
using RealEstateApp.Infrastructure.Data.Contexto;
using RealEstateApp.Infrastructure.Data.Repositorios;
using Xunit;

namespace RealEstateApp.Tests.Integracion.Repositorios
{
    public class RepositorioTipoPropiedadIntegracionTests
    {
        private readonly AplicacionDbContext _dbContext;
        private readonly RepositorioGenerico<TipoPropiedad> _repositorio;

        public RepositorioTipoPropiedadIntegracionTests()
        {
            var options = new DbContextOptionsBuilder<AplicacionDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext   = new AplicacionDbContext(options);
            _repositorio = new RepositorioGenerico<TipoPropiedad>(_dbContext);
        }

        [Fact]
        public async Task AgregarAsync_DebeGuardarTipoPropiedad_YObtenerseCorrectamente()
        {
            var tipo = new TipoPropiedad { Nombre = "Casa", Descripcion = "Vivienda unifamiliar" };

            var agregado = await _repositorio.AgregarAsync(tipo);
            var obtenido = await _repositorio.ObtenerPorIdAsync(agregado.Id);

            Assert.NotNull(obtenido);
            Assert.Equal("Casa",                 obtenido.Nombre);
            Assert.Equal("Vivienda unifamiliar", obtenido.Descripcion);
            Assert.True(agregado.Id > 0);
        }

        [Fact]
        public async Task ObtenerTodosAsync_DebeRetornarTodosLosTipos()
        {
            await _repositorio.AgregarAsync(new TipoPropiedad { Nombre = "Casa",        Descripcion = "Unifamiliar" });
            await _repositorio.AgregarAsync(new TipoPropiedad { Nombre = "Apartamento", Descripcion = "Residencial" });
            await _repositorio.AgregarAsync(new TipoPropiedad { Nombre = "Villa",       Descripcion = "Lujo" });

            var todos = await _repositorio.ObtenerTodosAsync();

            Assert.Equal(3, todos.Count());
        }

        [Fact]
        public async Task ActualizarAsync_DebeModificarElTipo()
        {
            var tipo = await _repositorio.AgregarAsync(
                new TipoPropiedad { Nombre = "Viejo", Descripcion = "Desc vieja" });

            tipo.Nombre      = "Loft";
            tipo.Descripcion = "Loft moderno";
            await _repositorio.ActualizarAsync(tipo);

            var actualizado = await _repositorio.ObtenerPorIdAsync(tipo.Id);
            Assert.Equal("Loft",         actualizado!.Nombre);
            Assert.Equal("Loft moderno", actualizado.Descripcion);
        }

        [Fact]
        public async Task EliminarAsync_DebeRemoverElTipo()
        {
            var tipo = await _repositorio.AgregarAsync(
                new TipoPropiedad { Nombre = "AEliminar", Descripcion = "Temp" });
            var id = tipo.Id;

            await _repositorio.EliminarAsync(tipo);
            var eliminado = await _repositorio.ObtenerPorIdAsync(id);

            Assert.Null(eliminado);
        }

        [Fact]
        public async Task ObtenerPorIdAsync_DebeRetornarNull_CuandoNoExiste()
        {
            var resultado = await _repositorio.ObtenerPorIdAsync(88888);
            Assert.Null(resultado);
        }
    }

    public class RepositorioTipoVentaIntegracionTests
    {
        private readonly AplicacionDbContext _dbContext;
        private readonly RepositorioGenerico<TipoVenta> _repositorio;

        public RepositorioTipoVentaIntegracionTests()
        {
            var options = new DbContextOptionsBuilder<AplicacionDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext   = new AplicacionDbContext(options);
            _repositorio = new RepositorioGenerico<TipoVenta>(_dbContext);
        }

        [Fact]
        public async Task AgregarAsync_DebeGuardarTipoVenta_YObtenerseCorrectamente()
        {
            var tipo = new TipoVenta { Nombre = "Venta", Descripcion = "Compraventa directa" };

            var agregado = await _repositorio.AgregarAsync(tipo);
            var obtenido = await _repositorio.ObtenerPorIdAsync(agregado.Id);

            Assert.NotNull(obtenido);
            Assert.Equal("Venta",              obtenido.Nombre);
            Assert.Equal("Compraventa directa", obtenido.Descripcion);
            Assert.True(agregado.Id > 0);
        }

        [Fact]
        public async Task ObtenerTodosAsync_DebeRetornarTodosLosTipos()
        {
            await _repositorio.AgregarAsync(new TipoVenta { Nombre = "Venta",   Descripcion = "Directa" });
            await _repositorio.AgregarAsync(new TipoVenta { Nombre = "Alquiler", Descripcion = "Mensual" });

            var todos = await _repositorio.ObtenerTodosAsync();

            Assert.Equal(2, todos.Count());
        }

        [Fact]
        public async Task ActualizarAsync_DebeModificarElTipo()
        {
            var tipo = await _repositorio.AgregarAsync(
                new TipoVenta { Nombre = "Viejo", Descripcion = "Desc vieja" });

            tipo.Nombre      = "Leasing";
            tipo.Descripcion = "Arrendamiento financiero";
            await _repositorio.ActualizarAsync(tipo);

            var actualizado = await _repositorio.ObtenerPorIdAsync(tipo.Id);
            Assert.Equal("Leasing",                 actualizado!.Nombre);
            Assert.Equal("Arrendamiento financiero", actualizado.Descripcion);
        }

        [Fact]
        public async Task EliminarAsync_DebeRemoverElTipo()
        {
            var tipo = await _repositorio.AgregarAsync(
                new TipoVenta { Nombre = "AEliminar", Descripcion = "Temp" });
            var id = tipo.Id;

            await _repositorio.EliminarAsync(tipo);
            var eliminado = await _repositorio.ObtenerPorIdAsync(id);

            Assert.Null(eliminado);
        }

        [Fact]
        public async Task ObtenerPorIdAsync_DebeRetornarNull_CuandoNoExiste()
        {
            var resultado = await _repositorio.ObtenerPorIdAsync(77777);
            Assert.Null(resultado);
        }

        [Fact]
        public async Task AgregarAsync_DebeAsignarIds_Diferentes_AMúltiplesTipos()
        {
            var t1 = await _repositorio.AgregarAsync(new TipoVenta { Nombre = "T1", Descripcion = "D1" });
            var t2 = await _repositorio.AgregarAsync(new TipoVenta { Nombre = "T2", Descripcion = "D2" });

            Assert.NotEqual(t1.Id, t2.Id);
        }
    }
}
