using Microsoft.AspNetCore.Identity;
using RealEstateApp.Core.Domain.Enumeraciones;
using RealEstateApp.Infrastructure.Identity.Entidades;

namespace RealEstateApp.Infrastructure.Identity.Semilla
{
    /// <summary>
    /// Semilla de datos para el sistema de identidad.
    /// Crea roles y usuarios de prueba para todos los roles del sistema.
    /// </summary>
    public static class SemillaIdentidad
    {
        public static async Task InicializarAsync(
            UserManager<UsuarioAplicacion> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // ROLES
            await CrearRolSiNoExisteAsync(roleManager, TipoRol.Administrador.ToString());
            await CrearRolSiNoExisteAsync(roleManager, TipoRol.Agente.ToString());
            await CrearRolSiNoExisteAsync(roleManager, TipoRol.Cliente.ToString());
            await CrearRolSiNoExisteAsync(roleManager, TipoRol.Desarrollador.ToString());

            // ADMINISTRADORES
            await CrearUsuarioSiNoExisteAsync(userManager, new UsuarioAplicacion
            {
                UserName       = "SuperAdmin",
                Email          = "super.admin@realestateapp.com",
                Nombre         = "Super",
                Apellido       = "Administrador",
                Cedula         = "00100000001",
                Telefono       = "8090000000",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                EstaActivo     = true,
                FechaRegistro  = new DateTime(2025, 1, 5, 8, 0, 0, DateTimeKind.Utc)
            }, "Admin123!", TipoRol.Administrador.ToString());

            await CrearUsuarioSiNoExisteAsync(userManager, new UsuarioAplicacion
            {
                UserName       = "adminpedro",
                Email          = "pedro.admin@realestateapp.com",
                Nombre         = "Pedro",
                Apellido       = "Ramírez",
                Cedula         = "00100000002",
                Telefono       = "8094444444",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                EstaActivo     = true,
                FechaRegistro  = new DateTime(2025, 1, 10, 9, 0, 0, DateTimeKind.Utc)
            }, "Admin123!", TipoRol.Administrador.ToString());

            // AGENTES  (5 agentes variados)
            await CrearUsuarioSiNoExisteAsync(userManager, new UsuarioAplicacion
            {
                UserName       = "albertsonagente",
                Email          = "albertson.agente@realestateapp.com",
                Nombre         = "Albertson",
                Apellido       = "Terrero",
                Telefono       = "8092222222",
                UrlFoto        = "/imagenes/perfiles/perfiles.png",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                EstaActivo     = true,
                FechaRegistro  = new DateTime(2025, 1, 15, 10, 0, 0, DateTimeKind.Utc)
            }, "Agente123!", TipoRol.Agente.ToString());

            await CrearUsuarioSiNoExisteAsync(userManager, new UsuarioAplicacion
            {
                UserName       = "geraldagente",
                Email          = "gerald.agente@realestateapp.com",
                Nombre         = "Gerald",
                Apellido       = "Gomera",
                Telefono       = "8095555555",
                UrlFoto        = "/imagenes/perfiles/perfiles.png",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                EstaActivo     = true,
                FechaRegistro  = new DateTime(2025, 1, 20, 11, 0, 0, DateTimeKind.Utc)
            }, "Agente123!", TipoRol.Agente.ToString());

            await CrearUsuarioSiNoExisteAsync(userManager, new UsuarioAplicacion
            {
                UserName       = "jesicaagente",
                Email          = "jesica.agente@realestateapp.com",
                Nombre         = "Jesica",
                Apellido       = "de la Rosa",
                Telefono       = "8096666666",
                UrlFoto        = "/imagenes/perfiles/perfiles.png",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                EstaActivo     = true,
                FechaRegistro  = new DateTime(2025, 2, 1, 9, 30, 0, DateTimeKind.Utc)
            }, "Agente123!", TipoRol.Agente.ToString());

            await CrearUsuarioSiNoExisteAsync(userManager, new UsuarioAplicacion
            {
                UserName       = "jhoanagente",
                Email          = "jhoan.agente@realestateapp.com",
                Nombre         = "Jhoan",
                Apellido       = "Paulino",
                Telefono       = "8097777777",
                UrlFoto        = "/imagenes/perfiles/perfiles.png",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                EstaActivo     = false, // Agente inactivo (para probar estadísticas)
                FechaRegistro  = new DateTime(2025, 2, 10, 14, 0, 0, DateTimeKind.Utc)
            }, "Agente123!", TipoRol.Agente.ToString());

            await CrearUsuarioSiNoExisteAsync(userManager, new UsuarioAplicacion
            {
                UserName       = "luisagente",
                Email          = "luis.agente@realestateapp.com",
                Nombre         = "Luis",
                Apellido       = "Castillo",
                Telefono       = "8098888888",
                UrlFoto        = "/imagenes/perfiles/perfiles.png",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                EstaActivo     = true,
                FechaRegistro  = new DateTime(2025, 2, 15, 8, 0, 0, DateTimeKind.Utc)
            }, "Agente123!", TipoRol.Agente.ToString());

            // CLIENTES  (4 clientes)
            await CrearUsuarioSiNoExisteAsync(userManager, new UsuarioAplicacion
            {
                UserName       = "leonardocliente",
                Email          = "leonardo.cliente@realestateapp.com",
                Nombre         = "Leonardo",
                Apellido       = "Taváres",
                Telefono       = "8091111111",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                EstaActivo     = true,
                FechaRegistro  = new DateTime(2025, 3, 1, 10, 0, 0, DateTimeKind.Utc)
            }, "Cliente123!", TipoRol.Cliente.ToString());

            await CrearUsuarioSiNoExisteAsync(userManager, new UsuarioAplicacion
            {
                UserName       = "sofiacliente",
                Email          = "sofia.cliente@realestateapp.com",
                Nombre         = "Sofía",
                Apellido       = "Torres",
                Telefono       = "8093333333",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                EstaActivo     = true,
                FechaRegistro  = new DateTime(2025, 3, 5, 11, 0, 0, DateTimeKind.Utc)
            }, "Cliente123!", TipoRol.Cliente.ToString());

            await CrearUsuarioSiNoExisteAsync(userManager, new UsuarioAplicacion
            {
                UserName       = "miguelcliente",
                Email          = "miguel.cliente@realestateapp.com",
                Nombre         = "Miguel",
                Apellido       = "Fernández",
                Telefono       = "8099999999",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                EstaActivo     = true,
                FechaRegistro  = new DateTime(2025, 3, 10, 14, 0, 0, DateTimeKind.Utc)
            }, "Cliente123!", TipoRol.Cliente.ToString());

            await CrearUsuarioSiNoExisteAsync(userManager, new UsuarioAplicacion
            {
                UserName       = "lauracliente",
                Email          = "laura.cliente@realestateapp.com",
                Nombre         = "Laura",
                Apellido       = "Núñez",
                Telefono       = "8090011111",
                EmailConfirmed = false, // Cliente aún no activado (para probar flujo)
                PhoneNumberConfirmed = false,
                EstaActivo     = false,
                FechaRegistro  = new DateTime(2025, 4, 1, 9, 0, 0, DateTimeKind.Utc)
            }, "Cliente123!", TipoRol.Cliente.ToString());

            // DESARROLLADORES  (2 desarrolladores)
            await CrearUsuarioSiNoExisteAsync(userManager, new UsuarioAplicacion
            {
                UserName       = "juanlopez",
                Email          = "juan.dev@realestateapp.com",
                Nombre         = "Juan",
                Apellido       = "López",
                Cedula         = "00200000001",
                Telefono       = "8090000001",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                EstaActivo     = true,
                FechaRegistro  = new DateTime(2025, 1, 3, 8, 0, 0, DateTimeKind.Utc)
            }, "Dev123!", TipoRol.Desarrollador.ToString());

            await CrearUsuarioSiNoExisteAsync(userManager, new UsuarioAplicacion
            {
                UserName       = "raiyydev",
                Email          = "raily.dev@realestateapp.com",
                Nombre         = "Raily",
                Apellido       = "Santos",
                Cedula         = "00200000002",
                Telefono       = "8090000002",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                EstaActivo     = false, // Dev inactivo
                FechaRegistro  = new DateTime(2025, 1, 4, 8, 0, 0, DateTimeKind.Utc)
            }, "Dev123!", TipoRol.Desarrollador.ToString());
        }

        // MÉTODOS AUXILIARES
        private static async Task CrearRolSiNoExisteAsync(
            RoleManager<IdentityRole> roleManager, string nombreRol)
        {
            if (!await roleManager.RoleExistsAsync(nombreRol))
                await roleManager.CreateAsync(new IdentityRole(nombreRol));
        }

        private static async Task CrearUsuarioSiNoExisteAsync(
            UserManager<UsuarioAplicacion> userManager,
            UsuarioAplicacion usuario,
            string contrasena,
            string rol)
        {
            if (await userManager.FindByEmailAsync(usuario.Email!) == null)
            {
                var resultado = await userManager.CreateAsync(usuario, contrasena);
                if (resultado.Succeeded)
                    await userManager.AddToRoleAsync(usuario, rol);
            }
        }
    }
}
