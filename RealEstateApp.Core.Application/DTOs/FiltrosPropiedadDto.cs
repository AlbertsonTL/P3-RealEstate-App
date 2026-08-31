using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateApp.Core.Application.DTOs
{
    public class FiltrosPropiedadDto
    {
        public string? CodigoBusqueda { get; set; }
        public int? TipoPropiedadId { get; set; }
        public decimal? PrecioMinimo { get; set; }
        public decimal? PrecioMaximo { get; set; }
        public int? CantidadHabitaciones { get; set; }
        public int? CantidadBanos { get; set; }
    }
}
