using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RealEstateApp.WebApp.Controllers;

[AllowAnonymous]
[Route("Error")]
public class ErrorController : Controller
{
    [HttpGet("{statusCode}")]
    public IActionResult HttpStatusCodeHandler(int statusCode)
    {
        return statusCode switch
        {
            403 => View("Error403"),
            404 => View("Error404"),
            _ => View("Error500")
        };
    }
}
