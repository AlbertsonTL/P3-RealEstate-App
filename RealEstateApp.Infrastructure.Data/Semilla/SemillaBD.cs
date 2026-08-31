using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RealEstateApp.Core.Domain.Entidades;
using RealEstateApp.Core.Domain.Enumeraciones;
using RealEstateApp.Infrastructure.Data.Contexto;

namespace RealEstateApp.Infrastructure.Data.Semilla
{
    public static class SemillaBD
    {
        public static async Task InicializarAsync(IServiceProvider servicios)
        {
            await SembrarCatalogosAsync(servicios);
        }

        public static async Task SembrarCatalogosAsync(IServiceProvider servicios)
        {
            var contexto = servicios.GetRequiredService<AplicacionDbContext>();
            await SembrarTiposPropiedadesAsync(contexto);
            await SembrarTiposVentasAsync(contexto);
            await SembrarMejorasAsync(contexto);
            await contexto.SaveChangesAsync();
        }

        public static async Task SembrarPropiedadesAsync(
            IServiceProvider servicios,
            List<string> agenteIds,
            List<string> clienteIds)
        {
            var contexto = servicios.GetRequiredService<AplicacionDbContext>();

            if (agenteIds.Count == 0 || clienteIds.Count == 0)
                return;

            if (contexto.Propiedades.Any())
                return;

            var idCasa         = contexto.TiposPropiedades.First(t => t.Nombre == "Casa").Id;
            var idApartamento  = contexto.TiposPropiedades.First(t => t.Nombre == "Apartamento").Id;

            var idVenta        = contexto.TiposVentas.First(t => t.Nombre == "Venta").Id;
            var idAlquiler     = contexto.TiposVentas.First(t => t.Nombre == "Alquiler").Id;

            var idPiscina      = contexto.Mejoras.First(m => m.Nombre == "Piscina").Id;
            var idParqueo      = contexto.Mejoras.First(m => m.Nombre == "Parqueo").Id;
            var idAire         = contexto.Mejoras.First(m => m.Nombre == "Aire Acondicionado").Id;

            string Cliente(int idx) => clienteIds[idx % clienteIds.Count];

            var tipoPropiedades = new[] { idCasa, idApartamento };
            var tipoVentas      = new[] { idVenta, idAlquiler };
            var mejorasDisponibles = new[] { idPiscina, idParqueo, idAire };

            var propiedades = new List<Propiedad>();
            var indicePropiedad = 0;

            for (var agenteIndex = 0; agenteIndex < agenteIds.Count; agenteIndex++)
            {
                var agenteId = agenteIds[agenteIndex];

                for (var propiedadIndex = 0; propiedadIndex < 3; propiedadIndex++)
                {
                    indicePropiedad++;
                    propiedades.Add(new Propiedad
                    {
                        Codigo               = $"100{indicePropiedad:000}",
                        TipoPropiedadId      = tipoPropiedades[(agenteIndex * 3 + propiedadIndex) % tipoPropiedades.Length],
                        TipoVentaId          = tipoVentas[(agenteIndex * 3 + propiedadIndex) % tipoVentas.Length],
                        Precio               = 3500000m + (indicePropiedad * 500000m),
                        Descripcion          = $"Propiedad número {indicePropiedad} gestionada por el agente asignado. Espacio versátil con acabados modernos y zonas comunes cercanas.",
                        TamañoMetros         = 70 + (propiedadIndex * 30) + (agenteIndex * 5),
                        CantidadHabitaciones = 1 + propiedadIndex,
                        CantidadBanos        = 1 + (propiedadIndex / 2),
                        Estado               = EstadoPropiedad.Disponible,
                        AgenteId             = agenteId,
                        FechaCreacion        = DateTime.UtcNow
                    });
                }
            }

            contexto.Propiedades.AddRange(propiedades);
            await contexto.SaveChangesAsync();

            var imagenes = new List<ImagenPropiedad>();
            foreach (var prop in propiedades)
            {
                imagenes.Add(new ImagenPropiedad 
                { 
                    PropiedadId = prop.Id, 
                    UrlImagen = "/imagenes/propiedades/propiedad.png", 
                    EsPrincipal = true 
                });
            }
            contexto.ImagenesPropiedades.AddRange(imagenes);
            await contexto.SaveChangesAsync();

            var ofertas = new List<Oferta>
            {
                new() { PropiedadId = propiedades[0].Id, ClienteId = Cliente(0), CifraOfertada = 11800000m, FechaOferta = DateTime.UtcNow, Estado = EstadoOferta.Pendiente }
            };
            contexto.Ofertas.AddRange(ofertas);
            await contexto.SaveChangesAsync();
        }

        public static async Task SembrarFavoritasYChatAsync(
            IServiceProvider servicios,
            List<string> clienteIds)
        {
            var contexto = servicios.GetRequiredService<AplicacionDbContext>();

            if (clienteIds.Count == 0)
                return;

            var propiedades = await contexto.Propiedades
                .AsNoTracking()
                .ToListAsync();

            if (propiedades.Count == 0)
                return;

            await SembrarFavoritasAsync(contexto, propiedades, clienteIds);
            await SembrarChatMensajesAsync(contexto, propiedades, clienteIds);
        }

        private static async Task SembrarFavoritasAsync(
            AplicacionDbContext contexto,
            List<Propiedad> propiedades,
            List<string> clienteIds)
        {
            if (contexto.PropiedadesFavoritas.Any())
                return;

            // Cada cliente marca como favoritas hasta 3 propiedades distintas.
            var favoritas = new List<PropiedadFavorita>();
            for (var clienteIndex = 0; clienteIndex < clienteIds.Count; clienteIndex++)
            {
                var clienteId = clienteIds[clienteIndex];
                var cantidadFavoritas = Math.Min(3, propiedades.Count);

                for (var i = 0; i < cantidadFavoritas; i++)
                {
                    var propiedad = propiedades[(clienteIndex + i) % propiedades.Count];

                    var yaExiste = favoritas.Any(f =>
                        f.PropiedadId == propiedad.Id && f.ClienteId == clienteId);

                    if (!yaExiste)
                    {
                        favoritas.Add(new PropiedadFavorita
                        {
                            PropiedadId = propiedad.Id,
                            ClienteId = clienteId
                        });
                    }
                }
            }

            contexto.PropiedadesFavoritas.AddRange(favoritas);
            await contexto.SaveChangesAsync();
        }

        private static async Task SembrarChatMensajesAsync(
            AplicacionDbContext contexto,
            List<Propiedad> propiedades,
            List<string> clienteIds)
        {
            if (contexto.ChatMensajes.Any())
                return;

            // Cada propiedad recibe una pequeña conversación entre un cliente y el
            // agente dueño de la propiedad, dejando el chat "en uso" con datos reales.
            var mensajes = new List<ChatMensaje>();
            var fechaBase = DateTime.UtcNow.AddDays(-2);

            for (var i = 0; i < propiedades.Count; i++)
            {
                var propiedad = propiedades[i];
                var clienteId = clienteIds[i % clienteIds.Count];
                var agenteId = propiedad.AgenteId;

                // Evita generar una conversación de un cliente consigo mismo si,
                // por casualidad, el id coincidiera con el del agente.
                if (clienteId == agenteId)
                    continue;

                var inicio = fechaBase.AddHours(i);

                mensajes.Add(new ChatMensaje
                {
                    PropiedadId = propiedad.Id,
                    RemitenteId = clienteId,
                    DestinatarioId = agenteId,
                    Contenido = $"Hola, me interesa la propiedad {propiedad.Codigo}. ¿Sigue disponible?",
                    FechaEnvio = inicio
                });

                mensajes.Add(new ChatMensaje
                {
                    PropiedadId = propiedad.Id,
                    RemitenteId = agenteId,
                    DestinatarioId = clienteId,
                    Contenido = "¡Hola! Sí, la propiedad sigue disponible. ¿Le gustaría coordinar una visita?",
                    FechaEnvio = inicio.AddMinutes(15)
                });

                mensajes.Add(new ChatMensaje
                {
                    PropiedadId = propiedad.Id,
                    RemitenteId = clienteId,
                    DestinatarioId = agenteId,
                    Contenido = "Perfecto, me gustaría visitarla este fin de semana.",
                    FechaEnvio = inicio.AddMinutes(30)
                });
            }

            contexto.ChatMensajes.AddRange(mensajes);
            await contexto.SaveChangesAsync();
        }

        private static async Task SembrarTiposPropiedadesAsync(AplicacionDbContext contexto)
        {
            if (contexto.TiposPropiedades.Any()) return;
            contexto.TiposPropiedades.AddRange(
                new TipoPropiedad { Nombre = "Casa",            Descripcion = "Propiedad de una o más plantas con terreno propio." },
                new TipoPropiedad { Nombre = "Apartamento",     Descripcion = "Unidad habitacional en un edificio de varias plantas." }
            );
        }

        private static async Task SembrarTiposVentasAsync(AplicacionDbContext contexto)
        {
            if (contexto.TiposVentas.Any()) return;
            contexto.TiposVentas.AddRange(
                new TipoVenta { Nombre = "Venta",              Descripcion = "Transferencia total de la propiedad mediante pago único." },
                new TipoVenta { Nombre = "Alquiler",           Descripcion = "Uso de la propiedad mediante pagos mensuales recurrentes." }
            );
        }

        private static async Task SembrarMejorasAsync(AplicacionDbContext contexto)
        {
            if (contexto.Mejoras.Any()) return;
            contexto.Mejoras.AddRange(
                new Mejora { Nombre = "Piscina",           Descripcion = "Estanque artificial techado o al aire libre para baño y natación." },
                new Mejora { Nombre = "Parqueo",           Descripcion = "Área designada y techada para el estacionamiento de vehículos." },
                new Mejora { Nombre = "Aire Acondicionado",Descripcion = "Sistema centralizado de climatización y control de temperatura." }
            );
        }
    }
}