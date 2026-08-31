using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using FluentValidation;

namespace RealEstateApp.WebApi.Middleware
{
    /// <summary>
    /// BUG FIX #3: Responde con ProblemDetails (RFC 7807) en lugar de objetos anónimos.
    /// Content-Type: application/problem+json
    /// </summary>
    public class ManejadorExcepcionesGlobal
    {
        private readonly RequestDelegate _next;

        public ManejadorExcepcionesGlobal(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await ManejarExcepcionAsync(context, ex);
            }
        }

        private static async Task ManejarExcepcionAsync(HttpContext context, Exception exception)
        {
            // RFC 7807 requiere este content-type
            context.Response.ContentType = "application/problem+json";

            ProblemDetails problema;

            switch (exception)
            {
                case ValidationException validationEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    problema = new ValidationProblemDetails(
                        validationEx.Errors
                            .GroupBy(e => e.PropertyName)
                            .ToDictionary(
                                g => g.Key ?? string.Empty,
                                g => g.Select(e => e.ErrorMessage).ToArray()))
                    {
                        Status = (int)HttpStatusCode.BadRequest,
                        Title = "Error de validación",
                        Detail = "Uno o más errores de validación han ocurrido.",
                        Instance = context.Request.Path
                    };
                    break;

                case UnauthorizedAccessException:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    problema = new ProblemDetails
                    {
                        Status = (int)HttpStatusCode.Unauthorized,
                        Title = "No autorizado",
                        Detail = "No tiene permisos para acceder a este recurso.",
                        Instance = context.Request.Path
                    };
                    break;

                case KeyNotFoundException:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    problema = new ProblemDetails
                    {
                        Status = (int)HttpStatusCode.NotFound,
                        Title = "Recurso no encontrado",
                        Detail = exception.Message,
                        Instance = context.Request.Path
                    };
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    problema = new ProblemDetails
                    {
                        Status = (int)HttpStatusCode.InternalServerError,
                        Title = "Error interno del servidor",
                        Detail = exception.Message,
                        Instance = context.Request.Path
                    };
                    break;
            }

            // Extensión estándar para correlacionar con logs
            problema.Extensions["traceId"] = context.TraceIdentifier;

            var opciones = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await context.Response.WriteAsync(JsonSerializer.Serialize(problema, opciones));
        }
    }
}
