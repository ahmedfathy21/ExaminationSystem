
using System.Net;
using System.Text.Json;
using ExaminationSystem.Common.Exceptions;
using ExaminationSystem.Common.Wrappers;

namespace ExaminationSystem.Common.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
            _logger.LogError(ex, ex.Message);
            context.Response.ContentType = "application/json";
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var (statusCode, response) = exception switch
        {
            ValidationException ve => (HttpStatusCode.UnprocessableEntity,ApiResponse.Fail(ve.Message,ve.ValidationErrors)),
            NotFoundException nfe  => (HttpStatusCode.NotFound,ApiResponse.Fail(nfe.Message)),
            UnauthorizedException ue => (HttpStatusCode.Unauthorized,ApiResponse.Fail(ue.Message)),
            ForbiddenException fe => (HttpStatusCode.Forbidden,ApiResponse.Fail(fe.Message)),
            ConflictException ce => (HttpStatusCode.Conflict,ApiResponse.Fail(ce.Message)),
            AppException ae => (HttpStatusCode.InternalServerError,ApiResponse.Fail("Internal Server Error")),
            _ => (HttpStatusCode.InternalServerError,ApiResponse.Fail("An unexpected error occurred,Please try again later."))
        };
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        await context.Response.WriteAsync(json);
        
    }
}