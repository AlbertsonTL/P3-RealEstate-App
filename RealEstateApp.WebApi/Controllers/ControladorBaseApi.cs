using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace RealEstateApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public abstract class ControladorBaseApi : ControllerBase
    {
        private IMediator? _mediador;
        protected IMediator Mediador => _mediador ??= HttpContext.RequestServices.GetService<IMediator>()!;
    }
}
