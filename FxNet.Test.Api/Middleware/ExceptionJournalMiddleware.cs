using System.Text;
using System.Text.Json;
using FxNet.Test.Api.Data;
using FxNet.Test.Api.Exceptions;
using FxNet.Test.Api.Models;

namespace FxNet.Test.Api.Middleware;

public class ExceptionJournalMiddleware
{
    private readonly RequestDelegate _next;
    private static long _eventIdCounter = DateTimeOffset.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;

    public ExceptionJournalMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AppDbContext db)
    {
        context.Request.EnableBuffering();
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var eventId = await LogExceptionAsync(context, db, ex);
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            var errorResponse = ex is SecureException
                ? new
                {
                    type = "Secure",
                    id = eventId.ToString(),
                    data = new { message = ex.Message }
                }
                : new
                {
                    type = "Exception",
                    id = eventId.ToString(),
                    data = new { message = $"Internal server error ID = {eventId}" }
                };

            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
    }

    private async Task<long> LogExceptionAsync(HttpContext context, AppDbContext db, Exception ex)
    {
        try
        {
            var parameters = new Dictionary<string, object?>();

            // Query parameters
            foreach (var (key, value) in context.Request.Query)
                parameters[key] = value.ToString();

            // Try to read body
            try
            {
                if (context.Request.ContentLength > 0 && context.Request.Body.CanSeek)
                {
                    context.Request.Body.Seek(0, SeekOrigin.Begin);
                    using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                    var body = await reader.ReadToEndAsync();
                    if (!string.IsNullOrEmpty(body))
                        parameters["_body"] = body;
                    context.Request.Body.Seek(0, SeekOrigin.Begin);
                }
            }
            catch (Exception bodyEx)
            {
                parameters["_bodyReadError"] = bodyEx.Message;
            }

            parameters["_path"] = context.Request.Path.Value;
            parameters["_method"] = context.Request.Method;

            var eventId = Interlocked.Increment(ref _eventIdCounter);

            var journal = new ExceptionJournal
            {
                EventId = eventId,
                CreatedAt = DateTime.UtcNow,
                Parameters = JsonSerializer.Serialize(parameters),
                StackTrace = ex.StackTrace,
                Text = ex.Message
            };

            db.ExceptionJournals.Add(journal);
            await db.SaveChangesAsync();

            return eventId;
        }
        catch
        {
            // Don't throw from exception handler
            return Interlocked.Increment(ref _eventIdCounter);
        }
    }
}
