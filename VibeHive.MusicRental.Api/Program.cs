using VibeHive.MusicRental.Api.Services;
using Grpc.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Rental records stay in this API. Album state belongs to Album Inventory.
builder.Services.AddSingleton<MusicService>();
builder.Services.AddSingleton<RentalManager>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Translate inventory errors once so both REST controllers return the same status codes.
app.Use(async (context, next) =>
{
    try
    {
        await next(context);
    }
    catch (RpcException error)
    {
        context.Response.StatusCode = error.StatusCode switch
        {
            StatusCode.InvalidArgument => StatusCodes.Status400BadRequest,
            StatusCode.NotFound => StatusCodes.Status404NotFound,
            StatusCode.FailedPrecondition or StatusCode.AlreadyExists => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status503ServiceUnavailable
        };
        string message = error.Status.Detail;
        if (context.Response.StatusCode == StatusCodes.Status503ServiceUnavailable)
        {
            message = "Album Inventory is unavailable. Start it, then refresh. "
                + "If a Pending rental appears in Active Rentals, return it before trying again.";
        }
        await context.Response.WriteAsJsonAsync(message);
    }
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
