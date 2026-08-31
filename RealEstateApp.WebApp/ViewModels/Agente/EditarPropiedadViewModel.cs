namespace RealEstateApp.WebApp.ViewModels.Agente;

public class EditarPropiedadViewModel : CrearPropiedadViewModel
{
    public int Id { get; set; }
    public List<ImagenPropiedadEdicionItem> ImagenesExistentes { get; set; } = [];
    public List<int> EliminarImagenIds { get; set; } = [];
}
