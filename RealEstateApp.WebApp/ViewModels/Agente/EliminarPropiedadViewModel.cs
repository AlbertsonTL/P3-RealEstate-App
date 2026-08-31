namespace RealEstateApp.WebApp.ViewModels.Agente;

public class EliminarPropiedadViewModel
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string TipoPropiedad { get; set; } = string.Empty;
    public string TipoVenta { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public string Descripcion { get; set; } = string.Empty;
}
