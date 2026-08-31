namespace RealEstateApp.WebApp.ViewModels.Admin;

/// <summary>
/// ViewModel para mostrar un tipo de catálogo (TipoPropiedad / TipoVenta)
/// junto con la cantidad de propiedades que lo referencian.
/// </summary>
public class TipoCatalogoConConteoViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int CantidadPropiedades { get; set; }
}
