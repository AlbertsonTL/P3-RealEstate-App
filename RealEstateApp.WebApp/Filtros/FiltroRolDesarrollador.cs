using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RealEstateApp.Infrastructure.Identity.Entidades;

namespace RealEstateApp.WebApp.Filtros;

/// <summary>
/// Si por alguna razón existiera una sesión de cookie activa
/// para un usuario con rol Desarrollador (los desarrolladores no deben tener
/// acceso a la WebApp, solo a la Api), se cierra esa sesión y se le redirige
/// a Inicio en vez de a Login, para evitar un bucle de redirecciones.
/// </summary>
public class FiltroRolDesarrollador : IAsyncActionFilter
{
    private readonly UserManager<UsuarioAplicacion> _userManager;
    private readonly SignInManager<UsuarioAplicacion> _signInManager;

    public FiltroRolDesarrollador(UserManager<UsuarioAplicacion> userManager, SignInManager<UsuarioAplicacion> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.HttpContext.User.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(context.HttpContext.User);
            if (user is not null && await _userManager.IsInRoleAsync(user, "Desarrollador"))
            {
                await _signInManager.SignOutAsync();

                if (context.Controller is Controller controller)
                {
                    controller.TempData["ErrorLogin"] = "No tienes permisos para acceder a esta aplicación";
                }

                // Redirigir a Inicio en vez de a Login evita el bucle infinito
                // (Login también tiene este filtro aplicado).
                context.Result = new RedirectToActionResult("Index", "Inicio", null);
                return;
            }
        }

        await next();
    }
}
