using BallastLaneTest.Application.Dtos;
using BallastLaneTest.Application.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace BallastLaneTest.Server.Endpoints;

/// <summary>
/// CRUD API endpoints for Record operations
/// </summary>
public static class RecordEndpoints
{
    public static void MapRecordEndpoints(this WebApplication app)
    {
        var recordGroup = app.MapGroup("/api/records")
            .WithTags("Records")
            .WithOpenApi()
            .RequireAuthorization("Bearer");

        recordGroup.MapGet("", GetRecords)
            .WithName("GetUserRecords")
            .WithSummary("Get all records for authenticated user")
            .Produces<IEnumerable<RecordDto>>(StatusCodes.Status200OK);

        recordGroup.MapGet("{id:int}", GetRecordById)
            .WithName("GetRecordById")
            .WithSummary("Get a specific record by ID")
            .Produces<RecordDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        recordGroup.MapPost("", CreateRecord)
            .WithName("CreateRecord")
            .WithSummary("Create a new record")
            .Produces<RecordDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        recordGroup.MapPut("{id:int}", UpdateRecord)
            .WithName("UpdateRecord")
            .WithSummary("Update an existing record")
            .Produces<RecordDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status403Forbidden);

        recordGroup.MapDelete("{id:int}", DeleteRecord)
            .WithName("DeleteRecord")
            .WithSummary("Delete a record")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status403Forbidden);
    }

    private static async Task<IResult> GetRecords(
        IRecordService recordService,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = GetUserIdFromContext(httpContext);
        if (userId <= 0)
            return Results.Unauthorized();

        var records = await recordService.GetUserRecordsAsync(userId, cancellationToken);
        return Results.Ok(records);
    }

    private static async Task<IResult> GetRecordById(
        int id,
        IRecordService recordService,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = GetUserIdFromContext(httpContext);
        if (userId <= 0)
            return Results.Unauthorized();

        var record = await recordService.GetRecordByIdAsync(id, cancellationToken);
        if (record is null)
            return Results.NotFound();

        // Verify ownership
        if (record.UserId != userId)
            return Results.Forbid();

        return Results.Ok(record);
    }

    private static async Task<IResult> CreateRecord(
        CreateRecordRequest request,
        IRecordService recordService,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = GetUserIdFromContext(httpContext);
        if (userId <= 0)
            return Results.Unauthorized();

        try
        {
            if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Content))
            {
                return Results.BadRequest(new { message = "Title and content are required." });
            }

            var record = await recordService.CreateRecordAsync(userId, request, cancellationToken);
            return Results.Created($"/api/records/{record.Id}", record);
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
    }

    private static async Task<IResult> UpdateRecord(
        int id,
        UpdateRecordRequest request,
        IRecordService recordService,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = GetUserIdFromContext(httpContext);
        if (userId <= 0)
            return Results.Unauthorized();

        try
        {
            if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Content))
            {
                return Results.BadRequest(new { message = "Title and content are required." });
            }

            var record = await recordService.UpdateRecordAsync(id, userId, request, cancellationToken);
            return Results.Ok(record);
        }
        catch (InvalidOperationException ex)
        {
            return Results.Forbid();
        }
    }

    private static async Task<IResult> DeleteRecord(
        int id,
        IRecordService recordService,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = GetUserIdFromContext(httpContext);
        if (userId <= 0)
            return Results.Unauthorized();

        try
        {
            var success = await recordService.DeleteRecordAsync(id, userId, cancellationToken);
            if (!success)
                return Results.NotFound();

            return Results.NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Results.Forbid();
        }
    }

    private static int GetUserIdFromContext(HttpContext context)
    {
        // Extract user ID from token/context
        // For now, this is a placeholder that should be implemented with proper authentication
        if (context.Items.TryGetValue("UserId", out var userId) && userId is int id)
        {
            return id;
        }

        // Try to get from query parameter for testing
        if (context.Request.Query.TryGetValue("userId", out var userIdQuery) && 
            int.TryParse(userIdQuery, out var idFromQuery))
        {
            return idFromQuery;
        }

        return 0;
    }
}
