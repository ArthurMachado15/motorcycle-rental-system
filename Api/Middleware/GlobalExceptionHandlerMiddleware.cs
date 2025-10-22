using System.Net;
using System.Text.Json;
using Application.Common.Exceptions;
using Application.DTOs;

namespace Api.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse();

        switch (exception)
        {
            case ValidationException validationException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Mensagem = "Dados inválidos";
                break;

            case NotFoundException notFoundException:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Mensagem = notFoundException.Message.Contains("Motorcycle") ? "Moto não encontrada" :
                                   notFoundException.Message.Contains("Courier") ? "Entregador não encontrado" :
                                   notFoundException.Message.Contains("Rental") ? "Locação não encontrada" :
                                   "Dados não encontrados";
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Mensagem = "Erro interno do servidor";
                break;
        }

        var json = JsonSerializer.Serialize(response);

        return context.Response.WriteAsync(json);
    }
}
